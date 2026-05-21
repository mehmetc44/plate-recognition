using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Application.Models;

namespace PlakaTanima.Alarm.Services;

public class HikvisionAlarmListener : ICameraAlarmListener
{
    public string Brand => "Hikvision";
    private readonly ILogger<HikvisionAlarmListener> _logger;
    private readonly IAlarmParser _parser;

    public HikvisionAlarmListener(ILogger<HikvisionAlarmListener> logger, IEnumerable<IAlarmParser> parsers)
    {
        _logger = logger;
        _parser = parsers.First(p => p.Brand == this.Brand);
    }

    public async Task StartListeningAsync(CameraConfig config, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{CameraName}] {Brand} Binary I/O stream işçisi başlatılıyor...", config.Name, Brand);
        string url = $"http://{config.Ip}/ISAPI/Event/notification/alertStream";

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var handler = new HttpClientHandler 
                { 
                    Credentials = new NetworkCredential(config.Username, config.Password), 
                    PreAuthenticate = true 
                };
                
                using var client = new HttpClient(handler);
                client.Timeout = Timeout.InfiniteTimeSpan; // Akışın sürekli açık kalması için

                _logger.LogInformation("[{CameraName}] Kamera akışına bağlanılıyor...", config.Name);
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("[{CameraName}] Canlı akış bağlantısı kuruldu. Araç geçişleri bekleniyor...", config.Name);

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                
                // Kameradan akan ham byte'ları biriktireceğimiz dinamik hafıza havuzu
                using var memoryStream = new MemoryStream();
                byte[] buffer = new byte[8192]; // 8KB'lık chunk'lar halinde okuma yapacağız
                int bytesRead;

                // Boundary (Sınır çizgisi) işaretçileri
                byte[] boundaryBytes = Encoding.UTF8.GetBytes("--boundary");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("--boundary--");

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    // Okunan chunk'ı ana hafızaya ekle
                    memoryStream.Write(buffer, 0, bytesRead);

                    // Hafızadaki veriyi tarayıp komple bir paket (araç geçişi) tamamlanmış mı bakıyoruz
                    byte[] currentData = memoryStream.ToArray();
                    
                    // Eğer akışta bir sonraki boundary başladıysa veya paket bittiyse (endBoundary varsa)
                    int endPos = FindPattern(currentData, endBoundaryBytes);
                    
                    // Bazı firmware'ler direkt ana boundary ile kapatır, o yüzden akıllı kontrol yapıyoruz
                    if (endPos == -1)
                    {
                        // Birden fazla "--boundary" biriktiyse, ikinci boundary'nin başlangıcı ilk paketin bittiğini gösterir
                        int firstBoundary = FindPattern(currentData, boundaryBytes);
                        if (firstBoundary != -1)
                        {
                            int secondBoundary = FindPattern(currentData, boundaryBytes, firstBoundary + boundaryBytes.Length);
                            if (secondBoundary != -1)
                            {
                                endPos = secondBoundary;
                            }
                        }
                    }

                    // Bir paket tam olarak yakalandıysa, onu ayırıp parser'a gönderiyoruz
                    if (endPos != -1)
                    {
                        byte[] fullPacketBytes = new byte[endPos];
                        Buffer.BlockCopy(currentData, 0, fullPacketBytes, 0, endPos);

                        try
                        {
                            // 🚀 DELEGE ETME: Listener byte'ı toplar, Parser'a teslim eder!
                            var alertData = _parser.ParseMultipart(fullPacketBytes, config.Name);
                            
                            // Ekrana temiz çıktı basalım
                            LogAlert(alertData);
                        }
                        catch (Exception parseEx)
                        {
                            _logger.LogError("[{CameraName}] Paket parse edilirken hata oluştu: {Message}", config.Name, parseEx.Message);
                        }

                        // İşlenen kısmı hafızadan temizle, kalan byte'ları (varsa bir sonraki paketin başı) koru
                        memoryStream.SetLength(0);
                        int remainingBytes = currentData.Length - endPos;
                        if (remainingBytes > 0)
                        {
                            memoryStream.Write(currentData, endPos, remainingBytes);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("[{CameraName}] Dinleme görevi iptal edildi.", config.Name);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError("[{CameraName}] Akış koptu veya hata oluştu: {Message}. 5 saniye sonra tekrar denenecek...", config.Name, ex.Message);
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private void LogAlert(CameraAlarmAlert alert)
    {
        Console.WriteLine("\n========================================================");
        _logger.LogInformation("🚨 [ARAÇ GEÇTİ VE PARSE EDİLDİ] - {Camera}", alert.CameraName.ToUpper());
        Console.WriteLine($"Plaka No        : {alert.PlateNumber}");
        Console.WriteLine($"Zaman           : {alert.EventTime}");
        Console.WriteLine($"Araç Tipi/Marka : {alert.VehicleType} / {alert.VehicleBrand}");
        Console.WriteLine($"Araç / Plaka Rk : {alert.VehicleColor} / {alert.PlateColor}");
        Console.WriteLine($"Kütüphane / Yön : {alert.ListLibraryName} / {alert.MovingDirection}");
        
        // Resimlerin başarıyla ayıklandığını kontrol edelim
        Console.WriteLine($"Geniş Açı Resim : {(alert.FullSceneImageBytes != null ? $"{alert.FullSceneImageBytes.Length} byte ✅" : "Yok ❌")}");
        Console.WriteLine($"Araç Yakın Resim: {(alert.VehicleImageBytes != null ? $"{alert.VehicleImageBytes.Length} byte ✅" : "Yok ❌")}");
        Console.WriteLine($"Plaka Resim     : {(alert.PlateImageBytes != null ? $"{alert.PlateImageBytes.Length} byte ✅" : "Yok ❌")}");
        Console.WriteLine("========================================================\n");
    }

    private int FindPattern(byte[] source, byte[] pattern, int startOffset = 0)
    {
        for (int i = startOffset; i <= source.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return i;
        }
        return -1;
    }
}
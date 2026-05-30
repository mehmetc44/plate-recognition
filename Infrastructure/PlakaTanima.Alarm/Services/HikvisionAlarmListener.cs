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
    private readonly IAlarmQueue _alarmQueue;

    public HikvisionAlarmListener(
        ILogger<HikvisionAlarmListener> logger, 
        IEnumerable<IAlarmParser> parsers,
        IAlarmQueue alarmQueue)
    {
        _logger = logger;
        _parser = parsers.First(p => p.Brand == this.Brand);
        _alarmQueue = alarmQueue;
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
                client.Timeout = Timeout.InfiniteTimeSpan; // Sürekli açık akış (Stream) için timeout devre dışı

                _logger.LogInformation("[{CameraName}] Kamera akışına bağlanılıyor...", config.Name);
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("[{CameraName}] Canlı akış bağlantısı kuruldu. Geçişler bekleniyor...", config.Name);

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var memoryStream = new MemoryStream();
                
                byte[] buffer = new byte[8192]; // 8KB'lık chunk'lar halinde okuma
                int bytesRead;

                byte[] boundaryBytes = Encoding.UTF8.GetBytes("--boundary");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("--boundary--");

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    memoryStream.Write(buffer, 0, bytesRead);

                    byte[] currentData = memoryStream.ToArray();
                    int endPos = FindPattern(currentData, endBoundaryBytes);
                    
                    if (endPos == -1)
                    {
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

                    // Bir paket tam olarak sınır çizgileriyle yakalandıysa
                    if (endPos != -1)
                    {
                        byte[] fullPacketBytes = new byte[endPos];
                        Buffer.BlockCopy(currentData, 0, fullPacketBytes, 0, endPos);

                        try
                        {
                            // Parser byte yığınını çözer ve nesneyi doldurur
                            var alertData = _parser.ParseMultipart(fullPacketBytes, config.Name);
                            
                            if (alertData != null && !string.IsNullOrEmpty(alertData.PlateNumber))
                            {
                                // 🚀 KAMERAYI ASLA YORMA: Veriyi kuyruğa at ve hemen bir sonraki pakete geç
                                await _alarmQueue.WriteAsync(alertData, cancellationToken);
                            }
                        }
                        catch (Exception parseEx)
                        {
                            _logger.LogError("[{CameraName}] Paket parse edilirken hata oluştu: {Message}", config.Name, parseEx.Message);
                        }

                        // Hafızada işlenen kısmı temizle, kalan byte'ları bir sonraki paket için koru
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
                _logger.LogError("[{CameraName}] Akış koptu: {Message}. 5 saniye sonra tekrar denenecek...", config.Name, ex.Message);
                await Task.Delay(5000, cancellationToken);
            }
        }
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
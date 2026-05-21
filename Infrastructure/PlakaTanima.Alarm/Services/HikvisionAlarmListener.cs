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
    private readonly IHttpClientFactory _httpClientFactory; // Alt çizgi burada tanımlı

    // Constructor içinde de ikisi temizce eşleniyor
    public HikvisionAlarmListener(ILogger<HikvisionAlarmListener> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartListeningAsync(CameraConfig config, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{CameraName}] {Brand} stream dinleyicisi başlatılıyor... IP: {Ip}", config.Name, Brand, config.Ip);

        string url = $"http://{config.Ip}/ISAPI/Event/notification/alertStream";

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Digest/Basic Authentication için handler mekanizması
                var handler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(config.Username, config.Password),
                    PreAuthenticate = true
                };

                // HttpClient'ı fabrikayı kullanarak güvenli şekilde üretiyoruz
                using var client = new HttpClient(handler);
                client.Timeout = Timeout.InfiniteTimeSpan; 

                _logger.LogInformation("[{CameraName}] Bağlantı kuruluyor: {Url}", config.Name, url);

                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("[{CameraName}] Bağlantı BAŞARILI! Canlı akış dinleniyor...", config.Name);

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream, Encoding.UTF8);

                while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                {
                    string? line = await reader.ReadLineAsync(cancellationToken);
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.Contains("<EventNotificationAlert") || line.Contains("Content-Length"))
                    {
                         _logger.LogInformation("[{CameraName}] [HAM VERİ SİNYALİ]: {Line}", config.Name, line);
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
                _logger.LogError(ex, "[{CameraName}] Akış koptu veya bağlanamadı! Tekrar denenecek...", config.Name);
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}
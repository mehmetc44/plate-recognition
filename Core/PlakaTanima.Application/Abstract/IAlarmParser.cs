using PlakaTanima.Application.Models;

namespace PlakaTanima.Application.Abstract;

public interface IAlarmParser
{
    /// <summary>
    /// Parser'ın hangi kamera markasına hizmet ettiğini belirtir (Örn: Hikvision, Dahua)
    /// </summary>
    string Brand { get; }

    /// <summary>
    /// Kameradan gelen HTTP Multipart mixed akışını (XML + Binary Resimler) tek seferde parse eder.
    /// </summary>
    /// <param name="multipartBytes">Kameradan akan ham byte bloğu</param>
    /// <param name="cameraName">Konfigürasyondaki varsayılan kamera adı (XML'de yoksa fallback olarak kullanılır)</param>
    /// <returns>Parse edilmiş zengin alarm nesnesi</returns>
    CameraAlarmAlert ParseMultipart(byte[] multipartBytes, string cameraName);
}
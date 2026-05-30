using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Application.Models;

namespace PlakaTanima.Infrastructure.Services;

public class MinioFileService : IFileService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioFileService(IMinioClient minioClient, IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }

    public async Task<string> UploadImageAsync(byte[] imageBytes, string folderName, string fileName, CancellationToken cancellationToken = default)
    {
        // Bu metodun eski imzasını koruyoruz, ancak asıl esnekliği yeni eklediğimiz 
        // overload (aşağıdaki metot) üzerinden sağlayacağız.
        return await UploadAlarmImageAsync(imageBytes, folderName, DateTime.Now, "UNKNOWN", fileName, cancellationToken);
    }

    // 🚀 Yeni eklenen ve sizin hiyerarşinizi kuran asıl metot:
    public async Task<string> UploadAlarmImageAsync(
        byte[] imageBytes, 
        string cameraName, 
        DateTime eventTime, 
        string plateNumber, 
        string imageType, // full, vehicle, plate
        CancellationToken cancellationToken = default)
    {
        if (imageBytes == null || imageBytes.Length == 0) return string.Empty;

        // 1. Klasör yapısındaki geçersiz karakterleri ve boşlukları temizleyelim (Örn: "30 AGUSTOS GIRIS" -> "30AgustosGiris")
        string cleanCameraName = string.Concat(cameraName.Where(c => !char.IsWhiteSpace(c)));
        string cleanPlate = string.Concat(plateNumber.Where(c => !char.IsWhiteSpace(c))).ToUpper();
        if (string.IsNullOrEmpty(cleanPlate)) cleanPlate = "UNKNOWN";

        // 2. Zaman kırılımlarını alıyoruz
        string year = eventTime.ToString("yyyy");
        string month = eventTime.ToString("MM");
        string day = eventTime.ToString("dd");
        string time = eventTime.ToString("HH-mm-ss"); // Dosya sistemleri ve URL güvenliği için ":" yerine "-" kullandık

        // 3. İstediğiniz Tam Yapı: 30AgustosGiris/2026/05/22/34ABC034/10-30-45/full.jpg
        string objectName = $"{cleanCameraName}/{year}/{month}/{day}/{cleanPlate}/{time}/{imageType}.jpg";

        using var memoryStream = new MemoryStream(imageBytes);

        // Bucket kontrolü ve oluşturulması
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.BucketName);
        bool exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        // MinIO'ya yükleme emri
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithStreamData(memoryStream)
            .WithObjectSize(memoryStream.Length)
            .WithContentType("image/jpeg");

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        // PostgreSQL'e kaydedilecek olan tam Object Path'i dönüyoruz
        return objectName;
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using PlakaTanima.Application.Services;

namespace PlakaTanima.Infrastructure.Services
{
    public class MinioStorageService : IMinioStorageService
    {
        private readonly IMinioClient _minioClient;

        public MinioStorageService()
        {
            // Resolve connection parameters from environment variables
            var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT") ?? "localhost:9000";
            var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY") ?? "minioadmin";
            var secretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY") ?? "minioadmin";
            var secureStr = Environment.GetEnvironmentVariable("MINIO_SECURE") ?? "False";
            var secure = secureStr.Equals("True", StringComparison.OrdinalIgnoreCase);

            // Configure the Minio Client
            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(secure)
                .Build();
        }

        public async Task<string> GetPresignedUrlAsync(string objectPath, int expirySeconds = 900)
        {
            if (string.IsNullOrWhiteSpace(objectPath))
                return string.Empty;

            try
            {
                // Mapped path format: platar-bucket/camera_name/year/month/day/hour/folder/plate.png
                var separatorIndex = objectPath.IndexOf('/');
                if (separatorIndex <= 0)
                {
                    // Fallback to direct path return if no bucket slash separator is present
                    return objectPath;
                }

                var bucketName = objectPath.Substring(0, separatorIndex);
                var objectKey = objectPath.Substring(separatorIndex + 1);

                var args = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithExpiry(expirySeconds);

                return await _minioClient.PresignedGetObjectAsync(args);
            }
            catch (Exception ex)
            {
                // Trace warning log and return empty string or fallback path
                Console.WriteLine($"[MINIO] Error generating presigned URL for {objectPath}: {ex.Message}");
                return string.Empty;
            }
        }
    }
}

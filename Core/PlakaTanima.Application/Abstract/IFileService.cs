namespace PlakaTanima.Application.Abstract;

public interface IFileService
{
    Task<string> UploadImageAsync(byte[] imageBytes, string folderName, string fileName, CancellationToken cancellationToken = default);
    
    // 🚀 Sizin klasör hiyerarşiniz için eklenen yeni sözleşme:
    Task<string> UploadAlarmImageAsync(byte[] imageBytes, string cameraName, DateTime eventTime, string plateNumber, string imageType, CancellationToken cancellationToken = default);
}
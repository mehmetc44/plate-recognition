using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraList
{
    public class CameraListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        
        // EKLENMESİ GEREKEN ALANLAR:
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string LocationName { get; set; }
        // Status tipiniz (enum veya string) ne ise o şekilde kalabilir
        public CameraStatus Status { get; set; } 
    }
}

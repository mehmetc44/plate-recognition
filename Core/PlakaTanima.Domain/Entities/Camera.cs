using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Domain.Entities;
public class Camera : BaseEntity
{
    public string Name { get; set; }
    public string GateName { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public int StreamChannel { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public CameraStatus Status { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}
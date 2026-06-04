using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Domain.Entities;

public class Camera : BaseEntity
{
    public string Name { get; set; } = null!;

    public Guid LocationId { get; set; }

    public Location Location { get; set; } = null!;

    public string IpAddress { get; set; } = null!;

    public int Port { get; set; } = 80;

    public int StreamChannel { get; set; } = 101;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public CameraStatus Status { get; set; } = CameraStatus.Offline;

    public DateTime? LastCheckedAt { get; set; }
}
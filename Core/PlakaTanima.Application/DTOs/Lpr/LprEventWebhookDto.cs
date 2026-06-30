using System;

namespace PlakaTanima.Application.DTOs.Lpr
{
    public class LprEventWebhookDto
    {
        public Guid EventId { get; set; }
        public string Plate { get; set; } = null!;
        public string CameraName { get; set; } = null!;
        public string Timestamp { get; set; } = null!;
        public bool HasImages { get; set; }
    }
}

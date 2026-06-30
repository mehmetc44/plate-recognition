using System;

namespace PlakaTanima.Application.DTOs.Lpr
{
    public class AnprEventHistoryDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = null!;
        public string CameraName { get; set; } = null!;
        public string EventTimestamp { get; set; } = null!;
        public string Direction { get; set; } = null!;
        public string PlateImg { get; set; } = null!;
        public string VehicleImg { get; set; } = null!;
        public string FullImg { get; set; } = null!;
    }
}

using System;

namespace PlakaTanima.Application.DTOs.Lpr
{
    public class AnprEventDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = null!;
        public string CameraName { get; set; } = null!;
        public string EventTimestamp { get; set; } = null!;
        public double Confidence { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleColor { get; set; }
        public string? VehicleBrand { get; set; }
        public string Direction { get; set; } = null!;
        public string? Country { get; set; }
        public string PlateImg { get; set; } = null!;
        public string VehicleImg { get; set; } = null!;
        public string FullImg { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Note { get; set; } = "";
    }
}

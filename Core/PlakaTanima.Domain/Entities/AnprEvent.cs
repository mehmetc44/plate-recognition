using System;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Domain.Entities;

public class AnprEvent : BaseEntity
{
    public string Plate { get; set; } = null!;

    public string CameraName { get; set; } = null!;

    public DateTime EventTimestamp { get; set; }

    public double Confidence { get; set; }

    public string VehicleType { get; set; } = "Unknown";

    public string VehicleColor { get; set; } = "Unknown";

    public string VehicleBrand { get; set; } = "Unknown";

    public string Direction { get; set; } = "Unknown";

    public string Country { get; set; } = "Turkey";

    // Image paths (relative to base storage folder)
    public string? PlateImagePath { get; set; }

    public string? VehicleImagePath { get; set; }

    public string? FullImagePath { get; set; }
}

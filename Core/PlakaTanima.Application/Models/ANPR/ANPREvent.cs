namespace PlakaTanima.Application.Models.ANPR;

public sealed class ANPREvent
{
    public Guid EventId { get; init; }

    public Guid CameraId { get; init; }

    public string CameraName { get; init; } = null!;

    public string PlateNumber { get; set; } = null!;

    public DateTime EventTime { get; set; }

    public string? Country { get; set; }

    public string? Direction { get; set; }

    public string? VehicleType { get; set; }

    public string? VehicleColor { get; set; }

    public string? VehicleBrand { get; set; }

    public double Confidence { get; set; }

    public byte[]? PlateImage { get; set; }

    public byte[]? VehicleImage { get; set; }

    public byte[]? FullImage { get; set; }
}
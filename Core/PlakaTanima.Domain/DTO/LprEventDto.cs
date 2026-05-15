using System.Text.Json;

public class LprEventDto
{
    public string EventId { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string CameraName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Confidence { get; set; }
    
    // Yeni Alanlar
    public string VehicleType { get; set; } = "Unknown";
    public string VehicleColor { get; set; } = "Unknown";
    public string VehicleBrand { get; set; } = "Unknown";
    public string Direction { get; set; } = "Unknown";
    public string Country { get; set; } = "Unknown";

    public LprImagesDto? Images { get; set; }
    public JsonElement? Raw { get; set; }
}

public class LprImagesDto
{
    public string? Plate { get; set; }   // Base64
    public string? Vehicle { get; set; } // Base64
    public string? Full { get; set; }    // Base64 (YENİ)
}
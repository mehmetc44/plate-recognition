using System;

namespace PlakaTanima.Domain.DTO;

public class LprEventDto
{
    public string EventId { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string CameraName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    
    // Altı çizilen Confidence burada!
    public decimal Confidence { get; set; } 
    
    // Resimler ve Ham veri için property'ler
    public LprImagesDto? Images { get; set; }
    public object? Raw { get; set; } 
}

public class LprImagesDto
{
    public string? Plate { get; set; }
    public string? Vehicle { get; set; }
}
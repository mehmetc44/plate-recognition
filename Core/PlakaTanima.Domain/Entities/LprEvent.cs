using System;
using System.Collections.Generic;

namespace PlakaTanima.Domain.Entities;

public class LprEvent
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string CameraName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    
    // Altı çizilen Confidence burada!
    public decimal Confidence { get; set; } 
    public DateTime CreatedAt { get; set; }
    
    // Navigation property'ler
    public ICollection<LprImage> Images { get; set; } = new List<LprImage>();
    public LprRawEvent? RawEvent { get; set; }
}
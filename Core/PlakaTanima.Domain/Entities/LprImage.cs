using System;

namespace PlakaTanima.Domain.Entities;

public class LprImage
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Type { get; set; } = string.Empty; 
    public string Path { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public LprEvent Event { get; set; } = null!;
}
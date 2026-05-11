using System;

namespace PlakaTanima.Domain.Entities;

public class LprRawEvent
{
    public Guid Id { get; set; }
    
    // Hangi event'e ait olduğunu tutan Foreign Key
    public Guid EventId { get; set; }
    
    // PostgreSQL'de JSONB ve TEXT olarak tutacağımız ham veriler
    public string RawJson { get; set; } = string.Empty;
    public string RawXml { get; set; } = string.Empty;

    // Navigation property (LprEvent ile One-to-One ilişki için)
    public LprEvent Event { get; set; } = null!;
}
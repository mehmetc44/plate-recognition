using System;

namespace PlakaTanima.Domain.Entities;

public class LprRawEvent
{
    public Guid Id { get; set; }
    
    public Guid EventId { get; set; }
    public string RawJson { get; set; } = string.Empty;
    public string RawXml { get; set; } = string.Empty;
    public LprEvent Event { get; set; } = null!;
}
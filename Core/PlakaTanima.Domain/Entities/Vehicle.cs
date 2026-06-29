using System;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string Plate { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Owner { get; set; } = null!;

    public VehicleCategory Category { get; set; }

    public string? Note { get; set; }
}

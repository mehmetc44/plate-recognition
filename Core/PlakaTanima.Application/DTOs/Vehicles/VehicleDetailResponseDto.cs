using System;

namespace PlakaTanima.Application.DTOs.Vehicles
{
    public class VehicleDetailResponseDto
    {
        public bool Exists { get; set; }
        public Guid? Id { get; set; }
        public string Plate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Note { get; set; } = "";
        public string Date { get; set; } = null!;
    }
}

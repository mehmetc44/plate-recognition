namespace PlakaTanima.Application.DTOs.Vehicles
{
    public class CreateVehicleDto
    {
        public string Plate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Note { get; set; }
    }
}

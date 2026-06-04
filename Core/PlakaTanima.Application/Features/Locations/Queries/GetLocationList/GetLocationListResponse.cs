namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationList;

public sealed class GetLocationListResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

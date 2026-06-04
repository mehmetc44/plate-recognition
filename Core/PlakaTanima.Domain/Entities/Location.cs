using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Camera> Cameras { get; set; }
        = new List<Camera>();
}
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.LocationRepositories;

public class LocationReadRepository : ReadRepository<Location>, ILocationReadRepository
{
    public LocationReadRepository(AppDbContext context) : base(context)
    {

    }
    public async Task<List<Location>> GetAllAsync()
    {
        return await Table
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
    public async Task<bool> ExistsByNameAsync(
    string name,
    Guid? excludeId = null)
    {
        var query = Table.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(x => x.Name == name);
    }
    public async Task<bool> HasCameraAsync(Guid locationId)
    {
        return await Table
            .AnyAsync(x =>
                x.Id == locationId &&
                x.Cameras.Any());
    }
    public async Task<List<Location>> GetAllWithCamerasAsync()
{
    return await Table
        .Include(x => x.Cameras)
        .AsNoTracking()
        .OrderBy(x => x.Name)
        .ToListAsync();
}
}
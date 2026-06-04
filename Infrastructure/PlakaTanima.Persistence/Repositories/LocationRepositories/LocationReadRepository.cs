using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.LocationRepositories;

public class LocationReadRepository: ReadRepository<Location> , ILocationReadRepository
{
    public LocationReadRepository(AppDbContext context): base(context)
    {
        
    }
    public async Task<List<Location>> GetAllAsync()
    {
        return await Table
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
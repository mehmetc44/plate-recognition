using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.LocationRepositories;

public class LocationWriteRepository
    : WriteRepository<Location>,
      ILocationWriteRepository
{
    public LocationWriteRepository(AppDbContext context)
        : base(context)
    {
    }
}
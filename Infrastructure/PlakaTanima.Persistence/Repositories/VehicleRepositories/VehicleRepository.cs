using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.VehicleRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.VehicleRepositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByPlateAsync(string plate, Guid? excludeId = null)
    {
        var query = Table.AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return await query.AnyAsync(x => x.Plate == plate);
    }
}

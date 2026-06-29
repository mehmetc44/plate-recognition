using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.CameraRepositories;

public class CameraRepository : Repository<Camera>, ICameraRepository
{
    public CameraRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Camera>> GetAllWithLocationAsync()
    {
        return await Table
            .Include(x => x.Location)
            .AsNoTracking()
            .OrderBy(x => x.Location.Name)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Camera?> GetCameraDetailAsync(Guid id)
    {
        return await Table
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsByIpAddressAsync(string ipAddress, Guid? excludeId = null)
    {
        var query = Table.AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return await query.AnyAsync(x => x.IpAddress == ipAddress);
    }
}

using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.CameraRepositories;

public class CameraWriteRepository
    : WriteRepository<Camera>,
      ICameraWriteRepository
{
    public CameraWriteRepository(AppDbContext context)
        : base(context)
    {
    }
}
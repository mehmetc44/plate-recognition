using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.CameraRepositories;

public class CameraReadRepository
    : ReadRepository<Camera>,
      ICameraReadRepository
{
    public CameraReadRepository(AppDbContext context)
        : base(context)
    {
    }
}
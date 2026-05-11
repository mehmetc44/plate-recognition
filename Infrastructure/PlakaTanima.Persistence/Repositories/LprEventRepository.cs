using System.Threading;
using System.Threading.Tasks;
using PlakaTanima.Application.Abstract.Repositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories;

public class LprEventRepository : ILprEventRepository
{
    private readonly AppDbContext _context;

    public LprEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LprEvent lprEvent, CancellationToken cancellationToken)
    {
        _context.LprEvents.Add(lprEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
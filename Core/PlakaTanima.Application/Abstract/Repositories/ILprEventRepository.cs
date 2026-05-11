using System.Threading;
using System.Threading.Tasks;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Abstract.Repositories;

public interface ILprEventRepository
{
    Task AddAsync(LprEvent lprEvent, CancellationToken cancellationToken);
}
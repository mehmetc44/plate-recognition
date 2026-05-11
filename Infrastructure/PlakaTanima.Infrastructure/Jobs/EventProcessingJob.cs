using System.Threading.Tasks;
using MediatR;
using PlakaTanima.Application.Abstract.Jobs;
using PlakaTanima.Application.Features.Commands;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.Infrastructure.Jobs;

public class EventProcessingJob : IEventProcessingJob
{
    private readonly IMediator _mediator;

    public EventProcessingJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProcessEventAsync(LprEventDto payload)
    {
        // Hangfire arka planda uyandı, payload'ı aldı ve MediatR'a fırlattı!
        var command = new CreateLprEventCommand(payload);
        await _mediator.Send(command);
    }
}
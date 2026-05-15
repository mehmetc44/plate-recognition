using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PlakaTanima.Application.Abstract.Jobs;
using PlakaTanima.Application.Abstract.Services;
using PlakaTanima.Application.Features.Commands;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.Infrastructure.Jobs;

public class EventProcessingJob : IEventProcessingJob
{
    private readonly IMediator _mediator;
    private readonly IPlateNotificationService _notificationService;

    public EventProcessingJob(IMediator mediator, IPlateNotificationService notificationService)
    {
        _mediator = mediator;
        _notificationService = notificationService;
    }

    public async Task ProcessEventAsync(LprEventDto payload)
    {
        // 1. Veriyi DB'ye kaydet (MediatR Handler halleder)
        await _mediator.Send(new CreateLprEventCommand(payload));

        // 2. UI'a sinyal gönder (Interface üzerinden)
        await _notificationService.SendNewPlateAsync(payload);
    }
}
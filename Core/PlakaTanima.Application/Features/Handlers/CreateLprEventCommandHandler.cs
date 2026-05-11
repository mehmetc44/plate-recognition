using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Application.Features.Commands;
using PlakaTanima.Application.Abstract.Repositories; // DbContext yerine Repository geldi!

namespace PlakaTanima.Application.Features.Handlers;

public class CreateLprEventCommandHandler : IRequestHandler<CreateLprEventCommand, Guid>
{
    private readonly ILprEventRepository _repository;

    public CreateLprEventCommandHandler(ILprEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateLprEventCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var newEvent = new LprEvent
        {
            Id = Guid.NewGuid(),
            Plate = payload.Plate ?? "UNKNOWN",
            CameraName = payload.CameraName ?? "UNKNOWN",
            Timestamp = payload.Timestamp != default ? payload.Timestamp : DateTime.UtcNow,
            Confidence = payload.Confidence,
            CreatedAt = DateTime.UtcNow
        };

        if (payload.Images != null)
        {
            if (!string.IsNullOrEmpty(payload.Images.Plate))
                newEvent.Images.Add(new LprImage { Id = Guid.NewGuid(), Type = "plate", Path = payload.Images.Plate, CreatedAt = DateTime.UtcNow });
            
            if (!string.IsNullOrEmpty(payload.Images.Vehicle))
                newEvent.Images.Add(new LprImage { Id = Guid.NewGuid(), Type = "vehicle", Path = payload.Images.Vehicle, CreatedAt = DateTime.UtcNow });
        }

        if (payload.Raw != null)
        {
            newEvent.RawEvent = new LprRawEvent
            {
                Id = Guid.NewGuid(),
                RawJson = System.Text.Json.JsonSerializer.Serialize(payload.Raw),
                RawXml = string.Empty 
            };
        }

        // Veritabanı işlemini Repository'e devrettik, EF Core'dan tamamen izole olduk!
        await _repository.AddAsync(newEvent, cancellationToken);

        return newEvent.Id;
    }
}
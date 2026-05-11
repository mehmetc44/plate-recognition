using System;
using MediatR;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.Application.Features.Commands;

// Bu komut çalıştığında bize kaydedilen Event'in Guid ID'sini dönecek
public class CreateLprEventCommand : IRequest<Guid>
{
    public LprEventDto Payload { get; set; }

    public CreateLprEventCommand(LprEventDto payload)
    {
        Payload = payload;
    }
}

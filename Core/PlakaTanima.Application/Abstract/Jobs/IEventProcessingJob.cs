using System;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.Application.Abstract.Jobs;

public interface IEventProcessingJob
{
    public Task ProcessEventAsync(LprEventDto payload);
}   

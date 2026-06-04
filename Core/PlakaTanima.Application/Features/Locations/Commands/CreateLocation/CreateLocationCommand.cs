using System;
using MediatR;

namespace PlakaTanima.Application.Features.Locations.Commands;

public sealed record CreateLocationCommand(string Name,string? Description) : IRequest<Guid>;

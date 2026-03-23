using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Domain.TicketTypes;

namespace Evently.Modules.Ticketing.Application.Events.CreateEvent;

internal sealed class CreateEventCommandHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateEventCommand>
{
    public async Task<Result<Success>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
            command.EventId,
            command.Title,
            command.Description,
            command.Location,
            command.StartsAtUtc,
            command.EndsAtUtc);

        eventRepository.Insert(@event);

        foreach (TicketTypeDto dto in command.TicketTypes)
        {
            var ticketType = TicketType.Create(dto.TicketTypeId, command.EventId, dto.Name, dto.Price, dto.Quantity);
            ticketTypeRepository.Insert(ticketType);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

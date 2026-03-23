using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Application.TicketTypes.CreateTicketType;

internal sealed class CreateTicketTypeCommandHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTicketTypeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTicketTypeCommand command, CancellationToken cancellationToken)
    {
        // business rule: ticket types can only be added to draft events
        Event? @event = await eventRepository.GetAsync(command.EventId, cancellationToken);
        if (@event is null)
            return EventErrors.NotFound(command.EventId);

        if (@event.Status != EventStatus.Draft)
            return EventErrors.NotDraft(command.EventId);

        var ticketType = TicketType.Create(
            Guid.NewGuid(),
            command.EventId,
            command.Name,
            command.Price,
            command.Quantity);

        ticketTypeRepository.Insert(ticketType);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ticketType.Id;
    }
}

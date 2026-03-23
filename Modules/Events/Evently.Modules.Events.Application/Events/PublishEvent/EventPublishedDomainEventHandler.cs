using Evently.Common.Domain.Events;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;
using Evently.Modules.Events.PublicApi;

namespace Evently.Modules.Events.Application.Events.PublishEvent;

internal sealed class EventPublishedDomainEventHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IEventPublisher eventPublisher) : IEventHandler<EventPublishedDomainEvent>
{
    public async Task HandleAsync(EventPublishedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Event? @event = await eventRepository.GetAsync(domainEvent.EventId, cancellationToken);
        if (@event is null)
        {
            return;
        }

        IReadOnlyList<TicketType> ticketTypes =
            await ticketTypeRepository.GetAllByEventIdAsync(domainEvent.EventId, cancellationToken);

        await eventPublisher.PublishAsync(
            new EventPublishedIntegrationEvent(
                @event.Id,
                @event.Title,
                @event.Description,
                @event.Location,
                @event.StartsAtUtc,
                @event.EndsAtUtc,
                ticketTypes.Select(t => new TicketTypeRecord(t.Id, t.EventId, t.Name, t.Price, t.Quantity)).ToList()),
            cancellationToken);
    }
}

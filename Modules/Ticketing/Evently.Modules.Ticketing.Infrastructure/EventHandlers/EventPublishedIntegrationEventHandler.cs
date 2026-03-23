using Evently.Common.Domain.Events;
using Evently.Modules.Events.PublicApi;
using Evently.Modules.Ticketing.Application.Events.CreateEvent;
using Waseet.CQRS;

namespace Evently.Modules.Ticketing.Infrastructure.EventHandlers;

internal sealed class EventPublishedIntegrationEventHandler(IMediator mediator)
    : IEventHandler<EventPublishedIntegrationEvent>
{
    public async Task HandleAsync(EventPublishedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new CreateEventCommand(
                integrationEvent.EventId,
                integrationEvent.Title,
                integrationEvent.Description,
                integrationEvent.Location,
                integrationEvent.StartsAtUtc,
                integrationEvent.EndsAtUtc),
            cancellationToken);
    }
}

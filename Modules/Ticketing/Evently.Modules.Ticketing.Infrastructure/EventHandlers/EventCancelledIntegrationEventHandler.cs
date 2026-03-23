using Evently.Common.Domain.Events;
using Evently.Modules.Events.PublicApi;
using Evently.Modules.Ticketing.Application.Events.CancelEvent;
using Waseet.CQRS;

namespace Evently.Modules.Ticketing.Infrastructure.EventHandlers;

internal sealed class EventCancelledIntegrationEventHandler(IMediator mediator) : IEventHandler<EventCancelledIntegrationEvent>
{
	public async Task HandleAsync(EventCancelledIntegrationEvent @event, CancellationToken cancellationToken)
	{
		await mediator.Send(new CancelEventCommand(@event.EventId), cancellationToken);
	}
}

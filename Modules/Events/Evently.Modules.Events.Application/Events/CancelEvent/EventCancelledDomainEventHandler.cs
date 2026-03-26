using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Events;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.PublicApi;

namespace Evently.Modules.Events.Application.Events.CancelEvent;

internal sealed class EventCancelledDomainEventHandler(IIntegrationEventPublisher eventPublisher) : IEventHandler<EventCancelledDomainEvent>
{
	public async Task HandleAsync(EventCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		await eventPublisher.PublishAsync(
			new EventCancelledIntegrationEvent(domainEvent.EventId),
			cancellationToken);
	}
}

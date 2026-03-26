using Evently.Common.Domain.Events;

namespace Evently.Common.Application.Messaging;

public interface IIntegrationEventPublisher
{
	Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
		  where TEvent : IEvent;
}

namespace Evently.Common.Infrastructure.MessageBroker;

public sealed class IntegrationEventConsumerOptions<TModule>
	 where TModule : class
{
	public string QueueName { get; set; } = string.Empty;
	public List<Type> IntegrationEventTypes { get; set; } = [];
}

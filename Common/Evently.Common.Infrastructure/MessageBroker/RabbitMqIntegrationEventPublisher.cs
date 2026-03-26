using System.Text;
using System.Text.Json;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Events;
using RabbitMQ.Client;

namespace Evently.Common.Infrastructure.MessageBroker;

public sealed class RabbitMqIntegrationEventPublisher(IConnection connection)
    : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var exchangeName = integrationEvent.GetType().Name;

        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()));

        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}

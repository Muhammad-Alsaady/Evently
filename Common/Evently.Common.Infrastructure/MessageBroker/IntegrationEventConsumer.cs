using System.Text;
using System.Text.Json;
using Evently.Common.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Evently.Common.Infrastructure.MessageBroker;

public sealed class IntegrationEventConsumer<TModule>(
	  IServiceScopeFactory serviceScopeFactory,
	  IConnection connection,
	  IOptions<IntegrationEventConsumerOptions<TModule>> options,
	  ILogger<IntegrationEventConsumer<TModule>> logger) : BackgroundService
	  where TModule : class
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

		var dlqExchange = $"{options.Value.QueueName}.dlq";
		await channel.ExchangeDeclareAsync(dlqExchange, ExchangeType.Fanout, durable: true,
			cancellationToken: stoppingToken);
		await channel.QueueDeclareAsync($"{options.Value.QueueName}.dlq", durable: true, exclusive: false,
			autoDelete: false, cancellationToken: stoppingToken);
		await channel.QueueBindAsync($"{options.Value.QueueName}.dlq", dlqExchange, string.Empty,
			cancellationToken: stoppingToken);

		// Declare the main queue with a pointer to the DLQ
		var queueArgs = new Dictionary<string, object?> { { "x-dead-letter-exchange", dlqExchange } };
		await channel.QueueDeclareAsync(options.Value.QueueName, durable: true, exclusive: false, autoDelete:
			false, arguments: queueArgs, cancellationToken: stoppingToken);

		// Bind queue to each integration event exchange
		foreach (var eventType in options.Value.IntegrationEventTypes)
		{
			await channel.ExchangeDeclareAsync(eventType.Name, ExchangeType.Fanout, durable: true,
			cancellationToken: stoppingToken);
			await channel.QueueBindAsync(options.Value.QueueName, eventType.Name, string.Empty,
			cancellationToken: stoppingToken);
		}

		var consumer = new AsyncEventingBasicConsumer(channel);

		consumer.ReceivedAsync += async (_, ea) =>
		{
			try
			{
				var eventTypeName = ea.Exchange;
				var eventType = options.Value.IntegrationEventTypes
					.FirstOrDefault(t => t.Name == eventTypeName);

				if (eventType is null)
				{
					logger.LogWarning("No registered type found for exchange {Exchange}", eventTypeName);
					await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
					return;
				}

				var json = Encoding.UTF8.GetString(ea.Body.Span);
				var integrationEvent = JsonSerializer.Deserialize(json, eventType);

				if (integrationEvent is null)
				{
					await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
					return;
				}

				await using var scope = serviceScopeFactory.CreateAsyncScope();
				var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

				await eventPublisher.PublishAsync((dynamic)integrationEvent, stoppingToken);

				await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing integration event from exchange {Exchange}",
					ea.Exchange);
				await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
			}
		};

		await channel.BasicConsumeAsync(options.Value.QueueName, autoAck: false, consumer: consumer,
			cancellationToken: stoppingToken);

		await Task.Delay(Timeout.Infinite, stoppingToken);
	}
}

using System.Text.Json;
using Evently.Common.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Evently.Common.Infrastructure.Outbox;

public sealed class OutboxProcessor<TDbContext>(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxProcessor<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessagesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var domainEventType = Type.GetType(message.Type);
                if (domainEventType is null)
                {
                    logger.LogWarning("Could not resolve type {Type} for outbox message {MessageId}", message.Type, message.Id);
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Content, domainEventType);
                if (domainEvent is null)
				{
					continue;
				}

				await eventPublisher.PublishAsync((dynamic)domainEvent, cancellationToken);

                message.ProcessedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                message.Error = ex.ToString();
                message.ProcessedAt = DateTime.UtcNow;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

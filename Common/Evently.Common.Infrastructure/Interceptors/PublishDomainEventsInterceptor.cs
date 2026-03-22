using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Evently.Common.Domain;
using Evently.Common.Domain.Events;

namespace Evently.Common.Infrastructure.Interceptors;

internal sealed class PublishDomainEventsInterceptor(IServiceScopeFactory serviceScopeFactory) : SaveChangesInterceptor
{
	public override async ValueTask<int> SavedChangesAsync(
		  SaveChangesCompletedEventData eventData,
		  int result,
		  CancellationToken cancellationToken = default)
	{
		if(eventData.Context is not null)
		{
			await PublishDomainEventsAsync(eventData.Context, cancellationToken);
		}

		return await base.SavedChangesAsync(eventData, result, cancellationToken);
	}

	private async Task PublishDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
	{
		var domainEvents = context.ChangeTracker
			.Entries<Entity>()
			.Select(e => e.Entity)
			.SelectMany(e =>
			{
				var events = e.DomainEvents.ToList();
				e.ClearDomainEvents();
				return events;
			})
			.ToList();

		using var scope = serviceScopeFactory.CreateScope();
		var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

		foreach (var domainEvent in domainEvents)
		{
			await eventPublisher.PublishAsync((dynamic)domainEvent, cancellationToken);
		}
	}
}

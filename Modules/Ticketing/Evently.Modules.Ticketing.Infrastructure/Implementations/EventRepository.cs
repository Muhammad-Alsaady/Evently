using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class EventRepository(TicketingDbContext context) : IEventRepository
{
	public async Task<Event?> GetAsync(Guid id)
	{
		return await context.Events.FindAsync(id);
	}

	public void Insert(Event @event) => context.Events.Add(@event);
}

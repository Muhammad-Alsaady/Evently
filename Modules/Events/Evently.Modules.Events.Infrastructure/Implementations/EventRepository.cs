using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Implementations;

internal sealed class EventRepository(EventsDbContext context) : IEventRepository
{
    public async Task<Event?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Events.FindAsync([id], cancellationToken);
    }

    public void Insert(Event @event)
    {
        context.Events.Add(@event);
    }
}

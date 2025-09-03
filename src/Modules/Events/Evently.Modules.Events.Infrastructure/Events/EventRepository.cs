using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.Events.Repository;
using Evently.Modules.Events.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Events;
internal sealed class EventRepository(EventsDbContext context) : IEventRepository
{
    public async Task<Event?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Events.FindAsync([id], cancellationToken);
    }

    public async Task Insert(Event @event)
    {
        await context.Events.AddAsync(@event);
    }
}

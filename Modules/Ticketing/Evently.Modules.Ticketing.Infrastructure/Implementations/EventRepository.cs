using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class EventRepository(TicketingDbContext context) : IEventRepository
{
    public void Insert(Event @event) => context.Events.Add(@event);
}

using Microsoft.EntityFrameworkCore;
using Evently.Modules.Events.Domain.TicketTypes;
using Evently.Modules.Events.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Implementations;

internal sealed class TicketTypeRepository(EventsDbContext context) : ITicketTypeRepository
{
    public async Task<TicketType?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.TicketTypes.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<TicketType>> GetAllByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await context.TicketTypes
            .Where(t => t.EventId == eventId)
            .ToListAsync(cancellationToken);
    }

    public void Insert(TicketType ticketType)
    {
        context.TicketTypes.Add(ticketType);
    }
}

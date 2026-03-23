using Evently.Modules.Ticketing.Domain.TicketTypes;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class TicketTypeRepository(TicketingDbContext context) : ITicketTypeRepository
{
    public void Insert(TicketType ticketType) => context.TicketTypes.Add(ticketType);
}

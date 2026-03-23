using Evently.Modules.Ticketing.Domain.TicketTypes;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class TicketTypeRepository(TicketingDbContext context) : ITicketTypeRepository
{
	public async Task<TicketType?> GetAsync(Guid id)
		=> await context.TicketTypes.FindAsync(id);

	public void Insert(TicketType ticketType) => context.TicketTypes.Add(ticketType);
}

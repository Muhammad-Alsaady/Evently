namespace Evently.Modules.Ticketing.Domain.TicketTypes;

public interface ITicketTypeRepository
{
	Task<TicketType?> GetAsync(Guid id);
    void Insert(TicketType ticketType);
}

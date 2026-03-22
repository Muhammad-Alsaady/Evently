namespace Evently.Modules.Events.Domain.TicketTypes;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketType>> GetAllByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    void Insert(TicketType ticketType);
}

namespace Evently.Modules.Events.PublicApi;

public interface IEventsApi
{
    Task<EventResponse?> GetAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<TicketTypeResponse?> GetTicketTypeAsync(Guid ticketTypeId, CancellationToken cancellationToken = default);
}

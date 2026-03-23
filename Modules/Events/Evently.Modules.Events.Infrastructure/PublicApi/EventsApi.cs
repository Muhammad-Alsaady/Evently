using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Modules.Events.PublicApi;

namespace Evently.Modules.Events.Infrastructure.PublicApi;

internal sealed class EventsApi(IDbConnectionFactory dbConnectionFactory) : IEventsApi
{
    public async Task<EventResponse?> GetAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, title, status
            FROM events.events
            WHERE id = @EventId
            """;

        return await connection.QuerySingleOrDefaultAsync<EventResponse>(sql, new { EventId = eventId });
    }

    public async Task<TicketTypeResponse?> GetTicketTypeAsync(Guid ticketTypeId, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, event_id AS EventId, name, price, quantity
            FROM events.ticket_types
            WHERE id = @TicketTypeId
            """;

        return await connection.QuerySingleOrDefaultAsync<TicketTypeResponse>(sql, new { TicketTypeId = ticketTypeId });
    }
}

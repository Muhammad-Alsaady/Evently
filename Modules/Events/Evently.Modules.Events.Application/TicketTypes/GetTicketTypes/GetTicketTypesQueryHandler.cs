using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.TicketTypes.GetTicketType;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketTypes;

internal sealed class GetTicketTypesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTicketTypesQuery, IReadOnlyList<TicketTypeResponse>>
{
    public async Task<Result<IReadOnlyList<TicketTypeResponse>>> Handle(
        GetTicketTypesQuery query,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, event_id AS EventId, name, price, quantity
            FROM events.ticket_types
            WHERE event_id = @EventId
            ORDER BY name
            """;

        IEnumerable<TicketTypeResponse> ticketTypes = await connection.QueryAsync<TicketTypeResponse>(
            sql,
            new { query.EventId });

        return ticketTypes.ToList().AsReadOnly();
    }
}

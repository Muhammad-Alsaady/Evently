using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Errors;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketType;

internal sealed class GetTicketTypeQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetTicketTypeQuery, TicketTypeResponse>
{
    public async Task<Result<TicketTypeResponse>> Handle(GetTicketTypeQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS TicketTypeId, event_id AS EventId, name, price, quantity
            FROM events.ticket_types
            WHERE id = @TicketTypeId
            """;

        TicketTypeResponse? ticketType = await connection.QuerySingleOrDefaultAsync<TicketTypeResponse>(
            sql,
            new { query.TicketTypeId });

        if (ticketType is null)
            return Error.NotFound("TicketTypes.NotFound", $"Ticket type with ID '{query.TicketTypeId}' was not found.");

        return ticketType;
    }
}

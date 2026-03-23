using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Errors;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Application.Events.GetEvent;

internal sealed class GetEventQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetEventQuery, EventResponse>
{
    public async Task<Result<EventResponse>> Handle(GetEventQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                e.id, e.category_id AS CategoryId, e.title, e.description,
                e.location, e.status, e.starts_at_utc AS StartsAtUtc, e.ends_at_utc AS EndsAtUtc,
                tt.id, tt.name, tt.price, tt.quantity
            FROM events.events e
            LEFT JOIN events.ticket_types tt ON tt.event_id = e.id
            WHERE e.id = @EventId
            """;

        // Dapper multi-mapping: one row per ticket type, aggregate into single EventResponse
        Dictionary<Guid, EventResponse> eventDictionary = [];

        await connection.QueryAsync<EventResponse, TicketTypeResponse, EventResponse>(
            sql,
            (eventRow, ticketType) =>
            {
                if (!eventDictionary.TryGetValue(eventRow.Id, out EventResponse? existingEvent))
                {
                    existingEvent = eventRow with { TicketTypes = [] };
                    eventDictionary[eventRow.Id] = existingEvent;
                }

                if (ticketType is not null)
                    ((List<TicketTypeResponse>)existingEvent.TicketTypes).Add(ticketType);

                return existingEvent;
            },
            new { query.EventId },
            splitOn: "id");

        EventResponse? result = eventDictionary.Values.SingleOrDefault();

        if (result is null)
            return Error.NotFound("Events.NotFound", $"Event with ID '{query.EventId}' was not found.");

        return result;
    }
}

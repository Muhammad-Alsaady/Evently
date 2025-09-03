using System.Data.Common;
using Dapper;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events.Models;
using MediatR;

namespace Evently.Modules.Events.Application.Events.GetEvent;

public sealed record GetEventQuery(Guid EventId) : IRequest<EventResponse?>;
internal sealed class GetEventQueryHandler(IDbConnectionFactory dbConnectionFactory) : IRequestHandler<GetEventQuery, EventResponse?>
{
    /*
      This handler:
        Opens a DB connection.
        Executes a SQL query to get an event and its ticket types.
        Uses Dapper to map the results into EventResponse with a collection of TicketTypeResponse.
        Handles one-to-many relationships by grouping ticket types under the same event.
        Returns the event if found, otherwise a NotFound error.
     */
    public async Task<EventResponse?> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        // Get Events and the related Ticket Types even the events doesn't have TicketTypes
        const string sql =
            $"""
             SELECT
                 e.id AS {nameof(EventResponse.Id)},
                 e.category_id AS {nameof(EventResponse.CategoryId)},
                 e.title AS {nameof(EventResponse.Title)},
                 e.description AS {nameof(EventResponse.Description)},
                 e.location AS {nameof(EventResponse.Location)},
                 e.starts_at_utc AS {nameof(EventResponse.StartsAtUtc)},
                 e.ends_at_utc AS {nameof(EventResponse.EndsAtUtc)},
                 tt.id AS {nameof(TicketTypeResponse.TicketTypeId)},
                 tt.name AS {nameof(TicketTypeResponse.Name)},
                 tt.price AS {nameof(TicketTypeResponse.Price)},
                 tt.currency AS {nameof(TicketTypeResponse.Currency)},
                 tt.quantity AS {nameof(TicketTypeResponse.Quantity)}
             FROM events.events e
             LEFT JOIN events.ticket_types tt ON tt.event_id = e.id
             WHERE e.id = @EventId
             """;

        // Dapper Mapping
        Dictionary<Guid, EventResponse> eventsDictionary = [];
        await connection.QueryAsync<EventResponse, TicketTypeResponse?, EventResponse>(
            sql,
            (@event, ticketType) =>
            {
                if (eventsDictionary.TryGetValue(@event.Id, out EventResponse? existingEvent))
                {
                    @event = existingEvent;
                }
                else
                {
                    eventsDictionary.Add(@event.Id, @event);
                }

                if (ticketType is not null)
                {
                    // If a row includes a ticket type, it’s added to the event’s TicketTypes list.
                    @event.TicketTypes.Add(ticketType);
                }

                return @event;
            },
            request,
            splitOn: nameof(TicketTypeResponse.TicketTypeId));

        if (!eventsDictionary.TryGetValue(request.EventId, out EventResponse eventResponse))
        {
            return Result.Failure<EventResponse>(EventErrors.NotFound(request.EventId));
        }

        return eventResponse;

    }
}

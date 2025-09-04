﻿using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketType;
internal sealed class TicketTypeQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<TicketTypeQuery, TicketTypeResponse?>
{
    public async Task<Result<TicketTypeResponse?>> Handle(TicketTypeQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
             SELECT
                 id AS {nameof(TicketTypeResponse.Id)},
                 event AS {nameof(TicketTypeResponse.EventId)},
                 Name AS {nameof(TicketTypeResponse.Name)},
                 price AS {nameof(TicketTypeResponse.Price)},
                 currency AS {nameof(TicketTypeResponse.Currency)},
                quantity AS {nameof(TicketTypeResponse.Quantity)},
             FROM events.ticket_types
             WHERE id = @TicketId
             """;


        TicketTypeResponse? ticketType =
            await connection.QuerySingleOrDefaultAsync<TicketTypeResponse>(sql, request);
        if (ticketType is null)
        {
            return Result.Failure<TicketTypeResponse>(TicketTypeError.NotFound(request.TicketTypeId));
        }
        return ticketType;
    }
}

using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Application.Events.GetEvents;

internal sealed class GetEventsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetEventsQuery, IReadOnlyList<EventResponse>>
{
    public async Task<Result<IReadOnlyList<EventResponse>>> Handle(
        GetEventsQuery query,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, category_id AS CategoryId, title, status,
                   starts_at_utc AS StartsAtUtc, ends_at_utc AS EndsAtUtc
            FROM events.events
            ORDER BY starts_at_utc
            """;

        IEnumerable<EventResponse> events = await connection.QueryAsync<EventResponse>(sql);

        return events.ToList().AsReadOnly();
    }
}

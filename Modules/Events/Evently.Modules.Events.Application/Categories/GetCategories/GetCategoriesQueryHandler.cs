using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Categories.GetCategory;

namespace Evently.Modules.Events.Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    public async Task<Result<IReadOnlyList<CategoryResponse>>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, name, is_archived AS IsArchived
            FROM events.categories
            ORDER BY name
            """;

        IEnumerable<CategoryResponse> categories = await connection.QueryAsync<CategoryResponse>(sql);

        return categories.ToList().AsReadOnly();
    }
}

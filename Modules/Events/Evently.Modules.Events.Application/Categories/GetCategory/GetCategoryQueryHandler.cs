using System.Data;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Errors;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Application.Categories.GetCategory;

internal sealed class GetCategoryQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCategoryQuery, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id, name, is_archived AS IsArchived
            FROM events.categories
            WHERE id = @CategoryId
            """;

        CategoryResponse? category = await connection.QuerySingleOrDefaultAsync<CategoryResponse>(
            sql,
            new { query.CategoryId });

        if (category is null)
		{
			return Error.NotFound("Categories.NotFound", $"Category with ID '{query.CategoryId}' was not found.");
		}

		return category;
    }
}

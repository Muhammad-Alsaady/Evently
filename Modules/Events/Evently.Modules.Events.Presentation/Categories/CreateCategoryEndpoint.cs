using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Categories.CreateCategory;

namespace Evently.Modules.Events.Presentation.Categories;

internal sealed class CreateCategoryEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("events/categories", Handle);
    }

    private static async Task<IResult> Handle(
        CreateCategoryRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateCategoryCommand(request.Name), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Created($"events/categories/{result.Value}", result.Value);
    }
}

internal sealed record CreateCategoryRequest(string Name);

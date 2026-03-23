using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Categories.UpdateCategory;

namespace Evently.Modules.Events.Presentation.Categories;

internal sealed class UpdateCategoryEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("events/categories/{id:guid}", Handle);
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateCategoryRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateCategoryCommand(id, request.Name), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok();
    }
}

internal sealed record UpdateCategoryRequest(string Name);

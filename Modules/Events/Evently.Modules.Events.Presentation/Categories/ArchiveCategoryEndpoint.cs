using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Categories.ArchiveCategory;

namespace Evently.Modules.Events.Presentation.Categories;

internal sealed class ArchiveCategoryEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("events/categories/{id:guid}", Handle);
    }

    private static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ArchiveCategoryCommand(id), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok();
    }
}

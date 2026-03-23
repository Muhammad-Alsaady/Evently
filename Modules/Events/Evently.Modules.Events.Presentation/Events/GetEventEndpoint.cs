using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Events.GetEvent;

namespace Evently.Modules.Events.Presentation.Events;

internal sealed class GetEventEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("events/{id:guid}", Handle);
    }

    private static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEventQuery(id), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok(result.Value);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Events.GetEvents;

namespace Evently.Modules.Events.Presentation.Events;

internal sealed class GetEventsEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("events", Handle);
    }

    private static async Task<IResult> Handle(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEventsQuery(), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok(result.Value);
    }
}

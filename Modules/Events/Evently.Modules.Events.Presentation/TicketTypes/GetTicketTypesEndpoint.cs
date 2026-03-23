using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.TicketTypes.GetTicketTypes;

namespace Evently.Modules.Events.Presentation.TicketTypes;

internal sealed class GetTicketTypesEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("events/{eventId:guid}/ticket-types", Handle);
    }

    private static async Task<IResult> Handle(
        Guid eventId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTicketTypesQuery(eventId), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok(result.Value);
    }
}

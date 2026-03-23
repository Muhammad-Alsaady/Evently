using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Events.RescheduleEvent;

namespace Evently.Modules.Events.Presentation.Events;

internal sealed class RescheduleEventEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("events/{id:guid}/reschedule", Handle);
    }

    private static async Task<IResult> Handle(
        Guid id,
        RescheduleEventRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RescheduleEventCommand(id, request.StartsAtUtc, request.EndsAtUtc),
            cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok();
    }
}

internal sealed record RescheduleEventRequest(DateTime StartsAtUtc, DateTime? EndsAtUtc);

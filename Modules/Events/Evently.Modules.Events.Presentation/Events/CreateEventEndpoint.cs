using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.Events.CreateEvent;

namespace Evently.Modules.Events.Presentation.Events;

internal sealed class CreateEventEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("events", Handle).WithTags("Events");
    }

    private static async Task<IResult> Handle(
        CreateEventRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateEventCommand(
                request.CategoryId,
                request.Title,
                request.Description,
                request.Location,
                request.StartsAtUtc,
                request.EndsAtUtc),
            cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Created($"events/{result.Value}", result.Value);
    }
}

internal sealed record CreateEventRequest(
    Guid CategoryId,
    string Title,
    string Description,
    string? Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc);

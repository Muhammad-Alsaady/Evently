using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.TicketTypes.CreateTicketType;

namespace Evently.Modules.Events.Presentation.TicketTypes;

internal sealed class CreateTicketTypeEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("events/{eventId:guid}/ticket-types", Handle);
    }

    private static async Task<IResult> Handle(
        Guid eventId,
        CreateTicketTypeRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateTicketTypeCommand(eventId, request.Name, request.Price, request.Quantity),
            cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Created($"events/ticket-types/{result.Value}", result.Value);
    }
}

internal sealed record CreateTicketTypeRequest(string Name, decimal Price, int Quantity);

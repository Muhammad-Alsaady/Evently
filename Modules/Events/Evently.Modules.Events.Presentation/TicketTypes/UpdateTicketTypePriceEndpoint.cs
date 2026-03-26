using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

namespace Evently.Modules.Events.Presentation.TicketTypes;

internal sealed class UpdateTicketTypePriceEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("events/ticket-types/{id:guid}/price", Handle).WithTags("Ticket Types");
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateTicketTypePriceRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateTicketTypePriceCommand(id, request.Price), cancellationToken);

        if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Ok();
    }
}

internal sealed record UpdateTicketTypePriceRequest(decimal Price);

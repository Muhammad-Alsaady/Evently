using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Ticketing.Application.Orders.CreateOrder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;

namespace Evently.Modules.Ticketing.Presentation.Orders;

internal sealed class CreateOrderEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("ticketing/orders", Handle);
	}

	private static async Task<IResult> Handle(
		CreateOrderRequest request,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var result = await mediator.Send(
			new CreateOrderCommand(request.EventId, request.CustomerId, request.TicketTypeId, request.Quantity),
			cancellationToken);

		if (result.IsError)
		{
			return result.Errors.ToProblem();
		}

		return Results.Created($"ticketing/orders/{result.Value}", result.Value);
	}
}

internal sealed record CreateOrderRequest(
	Guid EventId,
	Guid CustomerId,
	Guid TicketTypeId,
	int Quantity);

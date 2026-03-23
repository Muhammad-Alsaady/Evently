using Evently.Common.Domain.Errors;

namespace Evently.Modules.Ticketing.Domain.Orders;

public sealed class OrderErrors
{
	public static Error NotFound(Guid id) =>
	   Error.NotFound("Ticketing.Order.NotFound", $"Order with ID '{id}' was not found.");
}

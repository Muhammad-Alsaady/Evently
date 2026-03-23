using Evently.Common.Domain.Errors;

namespace Evently.Modules.Ticketing.Domain.Customers;

public sealed class CustomerErrors
{
	public static Error NotFound(Guid id) =>
	   Error.NotFound("Ticketing.Customer.NotFound", $"Customer with ID '{id}' was not found.");
}

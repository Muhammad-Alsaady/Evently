using Evently.Common.Domain.Errors;

namespace Evently.Modules.Ticketing.Domain.TicketTypes;

public sealed class TicketTypeErrors
{
	public static Error NotFound(Guid id) =>
	   Error.NotFound("Ticketing.TicketType.NotFound", $"Ticket type with ID '{id}' was not found.");
}

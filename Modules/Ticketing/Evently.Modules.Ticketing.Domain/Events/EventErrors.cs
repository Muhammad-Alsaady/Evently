using Evently.Common.Domain.Errors;

namespace Evently.Modules.Ticketing.Domain.Events;

public sealed class EventErrors
{
	public static Error NotFound(Guid id) =>
	   Error.NotFound("Ticketing.Events.NotFound", $"Event with ID '{id}' was not found.");
	public static Error AlreadyCancelled(Guid id) =>
		Error.Conflict("Ticketing.Events.AlreadyCancelled", $"Event with ID '{id}' is already cancelled.");
}

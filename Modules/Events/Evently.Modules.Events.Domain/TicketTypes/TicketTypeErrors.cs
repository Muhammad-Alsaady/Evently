using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.TicketTypes;

public static class TicketTypeErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("TicketTypes.NotFound", $"Ticket type with ID '{id}' was not found.");

    public static Error NotAvailable(Guid id) =>
        Error.Conflict("TicketTypes.NotAvailable", $"Ticket type with ID '{id}' is not available.");
}

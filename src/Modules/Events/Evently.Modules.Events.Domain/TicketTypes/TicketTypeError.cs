using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.TicketTypes;
public static class TicketTypeError
{
    public static Error NotFound(Guid ticketTypeId) => Error.NotFound("TicketType.NotFound", $"The ticket type with the identifier {ticketTypeId} was not found");

    public static readonly Error HasSamePrice = Error.Problem("TicketType.ChangePrice", "Ticket has the same price");
    public static Error TicketTypeExists(Guid ticketTypeId) => Error.Problem("TicketType.Exist", $"The ticket type with the identifier {ticketTypeId} already exists");
}

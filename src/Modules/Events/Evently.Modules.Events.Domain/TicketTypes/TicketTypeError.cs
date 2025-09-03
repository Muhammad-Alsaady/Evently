using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.TicketTypes;
public static class TicketTypeError
{
    public static readonly Error HasSamePrice = Error.Problem("TicketType.ChangePrice", "Ticket has the same price");
}

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketTypes;
public sealed record TicketTypeResponse(
    Guid Id,
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    decimal Quantity);

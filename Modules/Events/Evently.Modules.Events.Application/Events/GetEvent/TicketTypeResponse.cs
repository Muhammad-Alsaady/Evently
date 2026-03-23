namespace Evently.Modules.Events.Application.Events.GetEvent;

public sealed record TicketTypeResponse(Guid Id, string Name, decimal Price, int Quantity);

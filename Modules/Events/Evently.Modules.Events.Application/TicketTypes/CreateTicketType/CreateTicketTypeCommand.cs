using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.CreateTicketType;

public sealed record CreateTicketTypeCommand(
    Guid EventId,
    string Name,
    decimal Price,
    int Quantity) : ICommand<Guid>;

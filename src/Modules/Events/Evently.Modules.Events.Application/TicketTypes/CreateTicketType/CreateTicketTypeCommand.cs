using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;

namespace Evently.Modules.Events.Application.TicketTypes.CreateTicketType;
public sealed record CreateTicketTypeCommand(
    Guid EventId,
    string Name,
    decimal Price,
    string Currency,
    int Quantity) : ICommand<Result>;

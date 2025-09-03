using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

internal sealed record UpdateTicketTypePriceCommand(Guid TicketTypeId, decimal NewPrice) : ICommand;


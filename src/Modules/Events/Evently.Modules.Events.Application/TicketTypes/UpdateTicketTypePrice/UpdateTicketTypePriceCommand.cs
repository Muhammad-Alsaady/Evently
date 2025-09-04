using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

internal sealed record UpdateTicketTypePriceCommand(Guid TicketTypeId, decimal NewPrice) : ICommand;


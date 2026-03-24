using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;

[InvalidateCache("events:tickettype:{TicketTypeId}")]
public sealed record UpdateTicketTypePriceCommand(Guid TicketTypeId, decimal Price) : ICommand;

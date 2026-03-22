using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.TicketTypes;

public sealed record TicketTypePriceUpdatedDomainEvent(Guid TicketTypeId, decimal Price) : DomainEvent;

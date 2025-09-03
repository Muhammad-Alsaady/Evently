using Evently.Common.Domain.DomainEvents;

namespace Evently.Modules.Events.Domain.TicketTypes;
public sealed class TicketTypePriceChangedDomainEvent : DomainEvents
{
    public Guid TicketTypeId { get; set; }
    public decimal NewPrice { get; set; }
}

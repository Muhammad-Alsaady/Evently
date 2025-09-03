using Evently.Common.Domain.DomainEvents;

namespace Evently.Modules.Events.Domain.TicketTypes;
public sealed class TicketTypeCreatedDomainEvent : DomainEvents
{
    public Guid TicketTypeId { get; set; }
}

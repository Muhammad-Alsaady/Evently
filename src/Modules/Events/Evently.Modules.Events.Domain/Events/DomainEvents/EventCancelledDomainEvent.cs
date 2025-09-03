namespace Evently.Modules.Events.Domain.Events.DomainEvents;
public sealed class EventCancelledDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public Guid EventId { get; set; }
}

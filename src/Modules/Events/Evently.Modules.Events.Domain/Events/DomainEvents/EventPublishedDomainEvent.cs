namespace Evently.Modules.Events.Domain.Events.DomainEvents;
public sealed class EventPublishedDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public Guid EventId { get; set; }
}


namespace Evently.Modules.Events.Domain.Events.DomainEvents;

public sealed class EventCreatedDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public required Guid EventId { get; init; }
}

namespace Evently.Modules.Events.Domain.Events.DomainEvents;


public sealed class EventRescheduledDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public required Guid EventId { get; init; }
    public required DateTime StartsAtUtc { get; init; }
    public required DateTime? EndsAtUtc { get; init; }
}

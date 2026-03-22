using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Events;

public sealed record EventRescheduledDomainEvent(
    Guid EventId,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc) : DomainEvent;

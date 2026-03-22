using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Events;

public sealed record EventCancelledDomainEvent(Guid EventId) : DomainEvent;

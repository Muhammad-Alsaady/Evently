using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Events;

public sealed record EventPublishedDomainEvent(Guid EventId) : DomainEvent;

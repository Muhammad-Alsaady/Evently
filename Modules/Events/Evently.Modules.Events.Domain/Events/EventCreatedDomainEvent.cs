using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Events;

public sealed record EventCreatedDomainEvent(Guid EventId) : DomainEvent;

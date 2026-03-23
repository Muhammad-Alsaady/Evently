using Evently.Common.Domain.Events;

namespace Evently.Modules.Ticketing.Domain.Orders;

public sealed record OrderCreatedDomainEvent(Guid OrderId) : DomainEvent;

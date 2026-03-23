using Evently.Common.Domain.Events;

namespace Evently.Modules.Ticketing.PublicApi;

public sealed record OrderCreatedIntegrationEvent(Guid OrderId, Guid CustomerId, Guid EventId, decimal TotalPrice) : IEvent;


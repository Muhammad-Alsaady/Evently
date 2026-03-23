using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.PublicApi;

public sealed record EventCancelledIntegrationEvent(Guid EventId) : IEvent;

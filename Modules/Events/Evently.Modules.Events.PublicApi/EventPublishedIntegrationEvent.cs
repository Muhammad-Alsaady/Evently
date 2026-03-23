
using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.PublicApi;

public sealed record EventPublishedIntegrationEvent(
    Guid EventId,
    string Title,
    string Description,
    string? Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc) : IEvent;

using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.Events.GetEvents;

internal sealed record GetEventsQuery : IQuery<IReadOnlyCollection<EventResponse>>;

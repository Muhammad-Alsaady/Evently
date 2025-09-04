using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.Events.GetEvents;

internal sealed record GetEventsQuery : IQuery<IReadOnlyCollection<EventResponse>>;

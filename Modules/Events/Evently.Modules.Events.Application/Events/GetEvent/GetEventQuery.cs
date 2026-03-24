using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Events.GetEvent;

[Cache(Key = "events:event:{EventId}", Duration = 300)]
public sealed record GetEventQuery(Guid EventId) : IQuery<EventResponse>;

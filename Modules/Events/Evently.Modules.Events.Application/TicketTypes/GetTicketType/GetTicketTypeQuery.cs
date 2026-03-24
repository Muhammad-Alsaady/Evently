using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketType;

[Cache(Key = "events:tickettype:{TicketTypeId}", Duration = 300)]
public sealed record GetTicketTypeQuery(Guid TicketTypeId) : IQuery<TicketTypeResponse>;

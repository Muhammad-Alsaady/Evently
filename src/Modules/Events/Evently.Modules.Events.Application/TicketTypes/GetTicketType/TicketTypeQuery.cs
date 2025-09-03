using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketType;

internal sealed record TicketTypeQuery(Guid TicketTypeId) : IQuery<TicketTypeResponse?>;

using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketTypes;

internal sealed record GetTicketTypesQuery : IQuery<IReadOnlyCollection<TicketTypeResponse>>;


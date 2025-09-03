using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.TicketTypes.GetTicketTypes;

internal sealed record GetTicketTypesQuery : IQuery<IReadOnlyCollection<TicketTypeResponse>>;


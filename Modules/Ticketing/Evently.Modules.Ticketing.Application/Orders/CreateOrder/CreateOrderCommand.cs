
using Evently.Common.Application.Messaging;

namespace Evently.Modules.Ticketing.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid EventId, Guid CustomerId, Guid TicketTypeId, int Quantity) : ICommand;

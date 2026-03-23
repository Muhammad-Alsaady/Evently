using Evently.Modules.Ticketing.Domain.Orders;
using Evently.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class OrderRepository(TicketingDbContext context) : IOrderRepository
{
	public async Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken)
		=> await context.Orders.FindAsync([id], cancellationToken);

	public void Insert(Order order)
		=> context.Orders.Add(order);
}

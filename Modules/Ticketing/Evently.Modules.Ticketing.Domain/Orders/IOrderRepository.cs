namespace Evently.Modules.Ticketing.Domain.Orders;

public interface IOrderRepository
{
	void Insert(Order order);
	Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken);
}

namespace Evently.Modules.Ticketing.Domain.Orders;

public sealed class OrderItem
{
	public Guid Id { get; private set; }
	public Guid OrderId { get; private set; }
	public Guid TicketTypeId { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	private OrderItem()
	{
	}

	public static OrderItem Create(Guid id, Guid orderId, Guid ticketTypeId, int quantity, decimal price)
	{
		return new OrderItem
		{
			Id = id,
			OrderId = orderId,
			TicketTypeId = ticketTypeId,
			Quantity = quantity,
			Price = price,
		};
	}

}

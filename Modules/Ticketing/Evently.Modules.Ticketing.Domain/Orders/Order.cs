using System;
using System.Collections.Generic;
using System.Text;
using Evently.Common.Domain;

namespace Evently.Modules.Ticketing.Domain.Orders;

public sealed class Order : Entity
{
	public Guid Id { get; private set; }
	public Guid CustomerId { get; private set; }
	public Guid EventId { get; private set; }
	public OrderStatus OrderStatus { get; private set; }
	public decimal TotalPrice { get; private set; }
	private List<OrderItem> OrderItems { get; set; } = [];

	private Order()
	{
		
	}

	public static Order create(Guid id, Guid customerId, Guid eventId)
	{
		var order = new Order
		{
			Id = id,
			CustomerId = customerId,
			EventId = eventId,
		};

		order.Raise(new OrderCreateDomainEvent(order.Id));
		return order;
	}

	public static OrderItem AddItem(Guid ticketTypeId, int quantity, decimal price)
	{

		OrderItem.Create(Guid.NewGuid(), Id, ticketTypeId, quantity, price);
		OrderItems.Add(orderItem);
		return orderItem;
	}
}

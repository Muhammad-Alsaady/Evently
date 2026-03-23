using Evently.Common.Domain.Events;
using Evently.Modules.Ticketing.Domain.Orders;
using Evently.Modules.Ticketing.PublicApi;

namespace Evently.Modules.Ticketing.Application.Orders;

internal sealed class OrderCreatedDomainEventHandler(
	IOrderRepository orderRepository,
	IEventPublisher eventPublisher) : IEventHandler<OrderCreatedDomainEvent>
{
	public async Task HandleAsync(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		Order? order = await orderRepository.GetAsync(domainEvent.OrderId, cancellationToken);
		if (order is null)
		{
			return;
		}

		await eventPublisher.PublishAsync(
			new OrderCreatedIntegrationEvent(order.Id, order.CustomerId, order.EventId, order.TotalPrice),
			cancellationToken);
	}
}

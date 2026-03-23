using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Domain.Orders;
using Evently.Modules.Ticketing.Domain.TicketTypes;

namespace Evently.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
	ICustomerRepository customerRepository,
	ITicketTypeRepository ticketTypeRepository,
	IOrderRepository orderRepository,
	IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand>
{
	public async Task<Result<Success>> Handle(CreateOrderCommand request, CancellationToken cancellationToken = default)
	{
		var customer = await customerRepository.GetAsync(request.CustomerId);
		if (customer is null)
		{
			return CustomerErrors.NotFound(request.CustomerId);
		}

		var ticketType = await ticketTypeRepository.GetAsync(request.TicketTypeId);
		if (ticketType is null)
		{
			return TicketTypeErrors.NotFound(request.TicketTypeId);
		}

		var order = Order.Create(Guid.NewGuid(), request.CustomerId, request.EventId);
		order.AddItem(request.TicketTypeId, request.Quantity, ticketType.Price);

		orderRepository.Insert(order);
		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}

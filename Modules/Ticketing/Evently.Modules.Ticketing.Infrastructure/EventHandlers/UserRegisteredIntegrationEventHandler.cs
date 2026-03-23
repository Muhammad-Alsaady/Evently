using Evently.Common.Domain.Events;
using Evently.Modules.Ticketing.Application.Customers.CreateCustomer;
using Evently.Modules.Users.PublicApi;
using Waseet.CQRS;

namespace Evently.Modules.Ticketing.Infrastructure.EventHandlers;

internal sealed class UserRegisteredIntegrationEventHandler(IMediator mediator)
	 : IEventHandler<UserRegisteredIntegrationEvent>
{
	public async Task HandleAsync(UserRegisteredIntegrationEvent customer, CancellationToken cancellationToken)
	{
		await mediator.Send(new CreateCustomerCommand(customer.UserId, customer.Email, customer.FirstName, customer.LastName), cancellationToken);
	}
}

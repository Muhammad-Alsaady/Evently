using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal class CustomerRepository(TicketingDbContext context) : ICustomerRepository
{
	public void Insert(Customer customer, CancellationToken cancellationToken)
			=>context.Customers.Add(customer);
}

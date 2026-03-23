using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class CustomerRepository(TicketingDbContext context) : ICustomerRepository
{
	public void Insert(Customer customer)
			=>context.Customers.Add(customer);
}

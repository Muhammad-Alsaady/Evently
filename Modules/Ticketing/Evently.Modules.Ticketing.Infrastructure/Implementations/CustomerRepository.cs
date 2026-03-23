using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Infrastructure.Database;

namespace Evently.Modules.Ticketing.Infrastructure.Implementations;

internal sealed class CustomerRepository(TicketingDbContext context) : ICustomerRepository
{
	public async Task<Customer?> GetAsync(Guid customerId)
	{
		return await context.Customers.FindAsync(customerId);
	}

	public void Insert(Customer customer)
			=>context.Customers.Add(customer);
}

namespace Evently.Modules.Ticketing.Domain.Customers;

public interface ICustomerRepository
{
	Task<Customer?> GetAsync(Guid customerId);
	void Insert(Customer customer);
}

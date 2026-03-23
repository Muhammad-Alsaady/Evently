using Evently.Common.Domain;

namespace Evently.Modules.Ticketing.Domain.Customers;

public sealed class Customer : Entity
{
	public Guid Id { get; private set; }
	public Guid UserId { get; private set; }
	public string Email { get; private set; } = default!;
	public string FirstName { get; private set; } = default!;
	public string LastName { get; private set; } = default!;

	private Customer()
	{
		
	}

	public static Customer Create(Guid id, Guid userId, string email, string firstName, string lastName)
	{
		return new Customer
		{
			Id = id,
			UserId = userId,
			Email = email,
			FirstName = firstName,
			LastName = lastName
		};
	}
}

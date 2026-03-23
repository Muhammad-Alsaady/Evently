using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Customers;

namespace Evently.Modules.Ticketing.Application.Customers.CreateCustomer;

internal sealed class CreateCustomerCommandHandler(
	ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<CreateCustomerCommand>
{
	public async Task<Result<Success>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken = default)
	{
		var customer = Customer.Create(request.CustomerId, request.Email, request.FirstName, request.LastName);
		customerRepository.Insert(customer, cancellationToken);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success;
	}
}

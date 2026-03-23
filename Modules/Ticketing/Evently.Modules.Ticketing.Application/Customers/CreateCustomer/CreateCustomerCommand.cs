using Evently.Common.Application.Messaging;

namespace Evently.Modules.Ticketing.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(Guid UserId, string Email, string FirstName, string LastName) : ICommand;


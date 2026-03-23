using Evently.Common.Domain.Events;

namespace Evently.Modules.Users.PublicApi;

public sealed record UserRegisteredIntegrationEvent(Guid UserId,
	string Email,
	string FirstName,
	string LastName) : IEvent;

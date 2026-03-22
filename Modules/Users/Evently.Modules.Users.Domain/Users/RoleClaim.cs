using Microsoft.AspNetCore.Identity;

namespace Evently.Modules.Users.Domain.Users;

public class RoleClaim : IdentityRoleClaim<string>
{
	public Role Role { get; set; } = null!;
}

using Microsoft.AspNetCore.Identity;

namespace Evently.Modules.Users.Domain.Users;

public class UserRole : IdentityUserRole<string>
{
	public User User { get; set; } = null!;
	public Role Role { get; set; } = null!;
}

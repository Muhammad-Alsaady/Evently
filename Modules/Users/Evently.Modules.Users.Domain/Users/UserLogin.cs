using Microsoft.AspNetCore.Identity;

namespace Evently.Modules.Users.Domain.Users;

public class UserLogin : IdentityUserLogin<string>
{
	public User User { get; set; } = null!;
}

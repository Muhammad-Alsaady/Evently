using Evently.Common.Domain;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Domain.Tokens;

public class RefreshToken : IAuditableEntity
{
    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool Invalidated { get; set; }

    public string UserId { get; set; } = null!;

    public User User { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
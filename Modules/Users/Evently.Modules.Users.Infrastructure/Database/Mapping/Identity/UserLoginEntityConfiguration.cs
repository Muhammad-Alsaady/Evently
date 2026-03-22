using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Infrastructure.Database.Mapping.Identity;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("user_logins");
    }
}

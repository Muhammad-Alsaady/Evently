using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evently.Modules.Users.Domain.Users;

namespace Evently.Modules.Users.Infrastructure.Database.Mapping.Identity;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        
        builder.HasKey(x => new { x.UserId, x.RoleId });
    }
}

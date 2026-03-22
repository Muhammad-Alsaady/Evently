using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Evently.Common.Infrastructure.Database;

namespace Evently.Modules.Users.Infrastructure.Database;

public class UsersDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

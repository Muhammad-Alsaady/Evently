using Evently.Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

internal sealed class TicketingDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TicketingDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

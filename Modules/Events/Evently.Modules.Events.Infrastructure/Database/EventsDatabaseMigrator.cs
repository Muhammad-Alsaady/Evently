using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Evently.Common.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class EventsDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

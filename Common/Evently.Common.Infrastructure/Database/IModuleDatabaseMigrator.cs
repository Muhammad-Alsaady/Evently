using Microsoft.Extensions.DependencyInjection;

namespace Evently.Common.Infrastructure.Database;

public interface IModuleDatabaseMigrator
{
    Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default);
}

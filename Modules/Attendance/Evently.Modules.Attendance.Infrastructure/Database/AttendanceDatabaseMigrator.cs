using Evently.Common.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Modules.Attendance.Infrastructure.Database;

internal sealed class AttendanceDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AttendanceDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

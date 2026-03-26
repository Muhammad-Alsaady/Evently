using Evently.Modules.Attendance.Application.Abstractions.Data;

namespace Evently.Modules.Attendance.Infrastructure.Database;

internal sealed class UnitOfWork(AttendanceDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}

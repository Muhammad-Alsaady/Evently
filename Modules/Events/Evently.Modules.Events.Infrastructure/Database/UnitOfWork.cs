using Evently.Modules.Events.Application.Abstractions.Data;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class UnitOfWork(EventsDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}

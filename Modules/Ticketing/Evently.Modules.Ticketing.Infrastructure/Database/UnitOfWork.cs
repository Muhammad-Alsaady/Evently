using Evently.Modules.Ticketing.Application.Abstractions.Data;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

internal sealed class UnitOfWork(TicketingDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}

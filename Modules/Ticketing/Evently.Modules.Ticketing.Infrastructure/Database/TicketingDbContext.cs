using Evently.Modules.Ticketing.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

public sealed class TicketingDbContext(DbContextOptions<TicketingDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ticketing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketingDbContext).Assembly);
    }
}

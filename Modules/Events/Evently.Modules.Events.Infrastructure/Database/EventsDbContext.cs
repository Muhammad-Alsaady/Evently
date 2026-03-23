using Microsoft.EntityFrameworkCore;
using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Infrastructure.Database;

public sealed class EventsDbContext(DbContextOptions<EventsDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<TicketType> TicketTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("events");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventsDbContext).Assembly);
    }
}

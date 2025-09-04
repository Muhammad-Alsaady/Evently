using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Category.Models;
using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.TicketTypes;
using Microsoft.EntityFrameworkCore;

namespace Evently.Modules.Events.Infrastructure.Database;
public sealed class EventsDbContext(DbContextOptions<EventsDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<Event> Events { get; set; }
    internal DbSet<TicketType> TicketTypes { get; set; }
    internal DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Event);
    }
}

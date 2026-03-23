using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evently.Modules.Events.Infrastructure.Database;

internal sealed class EventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventsDbContext>();

        optionsBuilder
            .UseNpgsql("Host=localhost;Database=evently;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new EventsDbContext(optionsBuilder.Options);
    }
}

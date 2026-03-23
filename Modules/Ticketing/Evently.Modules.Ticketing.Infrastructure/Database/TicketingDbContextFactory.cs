using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evently.Modules.Ticketing.Infrastructure.Database;

internal sealed class TicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TicketingDbContext>();

        optionsBuilder
            .UseNpgsql("Host=localhost;Database=evently;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new TicketingDbContext(optionsBuilder.Options);
    }
}

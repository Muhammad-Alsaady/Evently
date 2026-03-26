using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evently.Modules.Attendance.Infrastructure.Database;

internal sealed class AttendanceDbContextFactory : IDesignTimeDbContextFactory<AttendanceDbContext>
{
    public AttendanceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AttendanceDbContext>();

        optionsBuilder
            .UseNpgsql("Host=localhost;Database=evently;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new AttendanceDbContext(optionsBuilder.Options);
    }
}

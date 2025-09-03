using Evently.Modules.Events.Application;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events.Repository;
using Evently.Modules.Events.Infrastructure.Data;
using Evently.Modules.Events.Infrastructure.Database;
using Evently.Modules.Events.Infrastructure.Events;
using Evently.Modules.Events.Presentation.Events;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Evently.Modules.Events.Infrastructure;
public static class EventModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        EventEndPointMapper.MapEndPoints(app);
    }

    public static IServiceCollection AddEventModule(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("EventsDb");

        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();

        services.TryAddSingleton(dataSource);
        services.AddDbContext<EventsDbContext>(options =>
        {
            options.UseNpgsql(connectionString, ngSqlOptions => ngSqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schema.Event))
                .UseSnakeCaseNamingConvention();
        });
        services.AddMediatR(op =>
        {
            op.RegisterServicesFromAssembly(AssemblyReference.Assembly);
        });
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EventsDbContext>());

        return services;
    }
}

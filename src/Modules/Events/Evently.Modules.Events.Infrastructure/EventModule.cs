using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Category.Repository;
using Evently.Modules.Events.Domain.Events.Repository;
using Evently.Modules.Events.Domain.TicketTypes;
using Evently.Modules.Events.Infrastructure.Categories;
using Evently.Modules.Events.Infrastructure.Database;
using Evently.Modules.Events.Infrastructure.Events;
using Evently.Modules.Events.Infrastructure.TicketTypes;
using Evently.Modules.Events.Presentation.Events;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Modules.Events.Infrastructure;
public static class EventModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        EventEndPointMapper.MapEndPoints(app);

    }

    public static IServiceCollection AddEventModule(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddInfraStructure(configuration);
        return services;
    }

    private static IServiceCollection AddInfraStructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("EventsDb");
        services.AddDbContext<EventsDbContext>(options =>
        {
            options.UseNpgsql(connectionString, ngSqlOptions => ngSqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schema.Event))
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EventsDbContext>());

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }

}

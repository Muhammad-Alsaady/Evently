using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Waseet.CQRS.Extensions;
using Evently.Common.Application;
using Evently.Common.Domain.Events;
using Evently.Common.Infrastructure.Database;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Modules.Events.Application;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;
using Evently.Modules.Events.Infrastructure.Database;
using Evently.Modules.Events.Infrastructure.Implementations;
using Evently.Modules.Events.Infrastructure.PublicApi;
using Evently.Modules.Events.PublicApi;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class EventsModuleExtensions
{
    public static IServiceCollection AddEventsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddDatabase(configuration);
        services.AddRepositories();

        return services;
    }

    private static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(AssemblyReference).Assembly;

        services.AddWaseet(assembly);

        services.AddApplicationBehaviors();

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IEventPublisher, EventPublisher>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddSingleton<PublishDomainEventsInterceptor>();

        services.AddDbContext<EventsDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<PublishDomainEventsInterceptor>();

            options
                .UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsHistoryTable("_migrations", "events"))
                .AddInterceptors(interceptor)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IModuleDatabaseMigrator, EventsDatabaseMigrator>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventsApi, EventsApi>();
    }
}

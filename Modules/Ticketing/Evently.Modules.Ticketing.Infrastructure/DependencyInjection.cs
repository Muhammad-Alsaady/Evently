using Evently.Common.Application;
using Evently.Common.Application.Extensions;
using Evently.Common.Domain.Events;
using Evently.Common.Infrastructure.Database;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Modules.Ticketing.Application;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Infrastructure.Database;
using Evently.Modules.Ticketing.Infrastructure.EventHandlers;
using Evently.Modules.Ticketing.Infrastructure.Implementations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Waseet.CQRS.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class TicketingModuleExtensions
{
    public static IServiceCollection AddTicketingModule(
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
        var applicationAssembly = typeof(AssemblyReference).Assembly;

        services.AddWaseet(applicationAssembly);
        services.AddApplicationBehaviors();
        services.AddValidatorsFromAssembly(applicationAssembly);

        // Integration event handlers live in Infrastructure — scan them separately
        services.RegisterHandlersFromAssemblyContaining(typeof(EventPublishedIntegrationEventHandler));
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddSingleton<PublishDomainEventsInterceptor>();

        services.AddDbContext<TicketingDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<PublishDomainEventsInterceptor>();

            options
                .UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsHistoryTable("_migrations", "ticketing"))
                .AddInterceptors(interceptor)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IModuleDatabaseMigrator, TicketingDatabaseMigrator>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}

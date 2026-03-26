using Evently.Common.Application.Extensions;
using Evently.Common.Infrastructure.Database;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Common.Infrastructure.MessageBroker;
using Evently.Common.Infrastructure.Outbox;
using Evently.Modules.Attendance.Application;
using Evently.Modules.Attendance.Application.Abstractions.Data;
using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Infrastructure.Database;
using Evently.Modules.Attendance.Infrastructure.EventHandlers;
using Evently.Modules.Attendance.Infrastructure.Implementations;
using Evently.Modules.Ticketing.PublicApi;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Waseet.CQRS.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AttendanceModuleExtensions
{
    public static IServiceCollection AddAttendanceModule(
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

        services.RegisterHandlersFromAssemblyContaining(typeof(OrderCreatedIntegrationEventHandler));

        services.Configure<IntegrationEventConsumerOptions<AssemblyReference>>(o =>
        {
            o.QueueName = "attendance";
            o.IntegrationEventTypes = [typeof(OrderCreatedIntegrationEvent)];
        });
        services.AddHostedService<IntegrationEventConsumer<AssemblyReference>>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddSingleton<PublishDomainEventsInterceptor>();

        services.AddDbContext<AttendanceDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<PublishDomainEventsInterceptor>();

            options
                .UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsHistoryTable("_migrations", "attendance"))
                .AddInterceptors(interceptor)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IModuleDatabaseMigrator, AttendanceDatabaseMigrator>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}

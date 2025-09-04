using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Infrastrucutre.Clock;
using Evently.Common.Infrastrucutre.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Evently.Common.Infrastrucutre;
public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(dataSource);
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}

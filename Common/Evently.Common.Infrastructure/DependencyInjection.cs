using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Evently.Common.Application.Data;
using Evently.Common.Infrastructure.Configuration;
using Evently.Common.Infrastructure.Data;
using Evently.Common.Infrastructure.Policies;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string[] activityModuleNames)
    {
        services.AddMemoryCache();

        services.AddHostOpenTelemetry(activityModuleNames);

        services.AddJwtAuthentication(configuration);
        services.AddClaimsAuthorization();
        services.AddDbConnectionFactory(configuration);

        return services;
    }

    private static IServiceCollection AddHostOpenTelemetry(
        this IServiceCollection services,
        params string[] activityModuleNames)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Evently"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddSource(activityModuleNames);

                tracing.AddOtlpExporter();
            });

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthConfiguration>()
            .Bind(configuration.GetSection(nameof(AuthConfiguration)));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["AuthConfiguration:Issuer"],
            ValidAudience = configuration["AuthConfiguration:Audience"],
#pragma warning disable S6781
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfiguration:Key"]  ?? throw new InvalidOperationException("JWT key is not configured")))
#pragma warning restore S6781
        };

        services.AddSingleton(tokenValidationParameters);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

        return services;
    }

    private static IServiceCollection AddClaimsAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<AuthorizationOptions>, AuthorizationConfigureOptions>();
        services.AddAuthorization();

        return services;
    }

	private static IServiceCollection AddDbConnectionFactory(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("Database")
			?? throw new InvalidOperationException("Database connection string 'Database' is not configured.");

		services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));

		return services;
	}
}

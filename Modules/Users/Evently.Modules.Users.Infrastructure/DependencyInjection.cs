using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Evently.Common.Infrastructure.Database;
using Evently.Common.Infrastructure.Policies;
using Evently.Modules.Users.Domain.Authentication;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Infrastructure.Authorization;
using Evently.Modules.Users.Infrastructure.Database;
using Evently.Modules.Users.Infrastructure.Policies;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
	public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDatabase(configuration);
		
		services.AddScoped<IClientAuthorizationService, ClientAuthorizationService>();
		
		services.AddSingleton<IPolicyFactory, UsersPolicyFactory>();

		return services;
	}

	private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("Postgres");

		services.AddDbContext<UsersDbContext>((provider, options) =>
		{
			var interceptor = provider.GetRequiredService<AuditableInterceptor>();

			options
				.UseNpgsql(connectionString, npgsqlOptions =>
				{
					npgsqlOptions.MigrationsHistoryTable(DbConsts.MigrationTableName, DbConsts.Schema);
				})
				.AddInterceptors(interceptor)
				.UseSnakeCaseNamingConvention();
		});
		
		services.AddScoped<IModuleDatabaseMigrator, UsersDatabaseMigrator>();
		
		services.AddSingleton<AuditableInterceptor>();

		services
			.AddIdentityCore<User>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;
			})
			.AddRoles<Role>()
			.AddEntityFrameworkStores<UsersDbContext>()
			.AddSignInManager()
			.AddDefaultTokenProviders();
	}
}

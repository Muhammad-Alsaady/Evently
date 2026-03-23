using Evently.Api.Seeding;
using Evently.Common.API.Extensions;
using Evently.Common.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebHostDependencies();

builder.AddCoreHostLogging();

builder.Services.AddCoreWebApiInfrastructure();

builder.Services.AddCoreInfrastructure(builder.Configuration,
[
    // Note: Shipments, Carriers, and Stocks modules are not implemented yet
    // Add their ActivityModuleNames here when those modules are created
]);

builder.Services
    .AddUsersModule(builder.Configuration)
    .AddEventsModule(builder.Configuration)
    .AddTicketingModule(builder.Configuration)
    .RegisterApiEndpointsFromAssemblyContaining(typeof(Evently.Modules.Events.Presentation.AssemblyReference))
    .RegisterApiEndpointsFromAssemblyContaining(typeof(Evently.Modules.Ticketing.Presentation.AssemblyReference));

// Seed entities in DEVELOPMENT mode
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<SeedService>();
}

var app = builder.Build();

// Run migrations in DEVELOPMENT mode
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.MigrateModuleDatabasesAsync();

    var userSeedService = scope.ServiceProvider.GetRequiredService<UserSeedService>();
    await userSeedService.SeedUsersAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseModuleMiddlewares();

app.MapApiEndpoints();

await app.RunAsync();

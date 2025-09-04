using Evently.Api.Extensions;
using Evently.Common.Application;
using Evently.Common.Infrastrucutre;
using Evently.Modules.Events.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication([Evently.Modules.Events.Application.AssemblyReference.Assembly]);
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("EventsDb")!);
builder.Services.AddEventModule(builder.Configuration);
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigration();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

EventModule.MapEndpoints(app);
await app.RunAsync();


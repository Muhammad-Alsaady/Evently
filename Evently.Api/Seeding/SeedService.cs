using System.Diagnostics.CodeAnalysis;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Evently.Api.Seeding;

public class SeedService(
    ILogger<SeedService> logger)
{
    public async Task SeedDataAsync()
    {
        logger.LogInformation("Data seeding is currently disabled - Carriers, Shipments, and Stocks modules are not implemented yet");
        await Task.CompletedTask;
    }
}

using System.Reflection;
using NetArchTest.Rules;

namespace Evently.Common.Tests.Architecture;

public class ModuleTests
{
	// Note: Shipments, Carriers, and Stocks modules are not implemented yet
	// Add their namespaces here when those modules are created
	// private const string UsersNamespace = "Modules.Users";
	// private const string ShipmentsNamespace = "Modules.Shipments";
	// private const string CarriersNamespace = "Modules.Carriers";
	// private const string StocksNamespace = "Modules.Stocks";

    [Fact]
    public void UsersModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    {
        var result = Types.InAssemblies(GetUsersModuleAssemblies())
            .Should()
            .NotHaveDependencyOnAny(
	            // Add other module namespaces here when they are created
	            "Modules.Carriers",
	            "Modules.Stocks",
                "Modules.Shipments")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }

    // Note: The following tests are commented out until the respective modules are implemented
    
    // [Fact]
    // public void CarriersModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    // {
    //     var result = Types.InAssemblies(GetCarriersModuleAssemblies())
    //         .Should()
    //         .NotHaveDependencyOnAny(
    //             "Modules.Users",
    //             "Modules.Stocks",
    //             "Modules.Shipments")
    //         .GetResult();
    //
    //     Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    // }

    // [Fact]
    // public void StocksModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    // {
    //     var result = Types.InAssemblies(GetStocksModuleAssemblies())
    //         .Should()
    //         .NotHaveDependencyOnAny(
    //             "Modules.Users",
    //             "Modules.Carriers",
    //             "Modules.Shipments")
    //         .GetResult();
    //
    //     Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    // }

    // [Fact]
    // public void ShipmentsModule_ShouldOnlyDependOn_CarriersAndStocksModules()
    // {
    //     var result = Types.InAssemblies(GetShipmentsModuleAssemblies())
    //         .Should()
    //         .NotHaveDependencyOnAny("Modules.Users")
    //         .GetResult();
    //
    //     Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    // }

    // [Fact]
    // public void ShipmentsModule_ShouldOnlyReference_PublicApiProjects()
    // {
    //     var shipmentsAssemblies = GetShipmentsModuleAssemblies();
    //
    //     // Check that Shipments module doesn't reference internal Carriers projects
    //     var carriersInternalResult = Types.InAssemblies(shipmentsAssemblies)
    //         .Should()
    //         .NotHaveDependencyOnAny(
    //             "Modules.Carriers.Domain",
    //             "Modules.Carriers.Infrastructure",
    //             "Modules.Carriers.Features")
    //         .GetResult();
    //
    //     Assert.True(carriersInternalResult.IsSuccessful,
    //         $"Shipments module should not reference internal Carriers projects: {string.Join(", ", carriersInternalResult.FailingTypeNames ?? [])}");
    //
    //     // Check that Shipments module doesn't reference internal Stocks projects
    //     var stocksInternalResult = Types.InAssemblies(shipmentsAssemblies)
    //         .Should()
    //         .NotHaveDependencyOnAny(
    //             "Modules.Stocks.Domain",
    //             "Modules.Stocks.Infrastructure",
    //             "Modules.Stocks.Features")
    //         .GetResult();
    //
    //     Assert.True(stocksInternalResult.IsSuccessful,
    //         $"Shipments module should not reference internal Stocks projects: {string.Join(", ", stocksInternalResult.FailingTypeNames ?? [])}");
    //
    //     // Verify that it DOES reference the PublicApi projects (optional positive test)
    //     var hasCarriersPublicApiDep = Types.InAssemblies(shipmentsAssemblies)
    //         .That()
    //         .HaveDependencyOn("Modules.Carriers.PublicApi")
    //         .GetTypes()
    //         .Any();
    //
    //     var hasStocksPublicApiDep = Types.InAssemblies(shipmentsAssemblies)
    //         .That()
    //         .HaveDependencyOn("Modules.Stocks.PublicApi")
    //         .GetTypes()
    //         .Any();
    //
    //     Assert.True(hasCarriersPublicApiDep || hasStocksPublicApiDep,
    //         "Shipments module should reference at least one PublicApi project");
    // }

    private static Assembly[] GetUsersModuleAssemblies()
    {
        return [
            ModuleAssemblies.UsersDomainAssembly,
            ModuleAssemblies.UsersInfrastructureAssembly,
            ModuleAssemblies.UsersFeaturesAssembly
        ];
    }

    // Note: The following helper methods are commented out until the respective modules are implemented
    
    // private static Assembly[] GetCarriersModuleAssemblies()
    // {
    //     return [
    //         ModuleAssemblies.CarriersDomainAssembly,
    //         ModuleAssemblies.CarriersInfrastructureAssembly,
    //         ModuleAssemblies.CarriersFeaturesAssembly,
    //         ModuleAssemblies.CarriersPublicApiAssembly
    //     ];
    // }

    // private static Assembly[] GetStocksModuleAssemblies()
    // {
    //     return [
    //         ModuleAssemblies.StocksDomainAssembly,
    //         ModuleAssemblies.StocksInfrastructureAssembly,
    //         ModuleAssemblies.StocksFeaturesAssembly,
    //         ModuleAssemblies.StocksPublicApiAssembly
    //     ];
    // }

    // private static Assembly[] GetShipmentsModuleAssemblies()
    // {
    //     return [
    //         ModuleAssemblies.ShipmentsDomainAssembly,
    //         ModuleAssemblies.ShipmentsInfrastructureAssembly,
    //         ModuleAssemblies.ShipmentsFeaturesAssembly
    //     ];
    // }
}

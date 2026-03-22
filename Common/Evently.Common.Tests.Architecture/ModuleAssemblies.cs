using System.Reflection;

namespace Evently.Common.Tests.Architecture;

internal static class ModuleAssemblies
{
    // Users module assemblies
    internal static readonly Assembly UsersDomainAssembly = Evently.Modules.Users.Domain.AssemblyReference.Assembly;
    internal static readonly Assembly UsersFeaturesAssembly = Evently.Modules.Users.Features.AssemblyReference.Assembly;
    internal static readonly Assembly UsersInfrastructureAssembly = Evently.Modules.Users.Infrastructure.AssemblyReference.Assembly;
}

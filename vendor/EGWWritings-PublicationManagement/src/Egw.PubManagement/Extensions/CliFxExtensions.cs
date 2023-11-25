using System.Reflection;

using CliFx;

using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary> CliFX extensions </summary>
public static class CliFxExtensions
{
    /// <summary> Add CliFx commands from assemblies </summary>
    /// <param name="self">Self</param>
    /// <param name="assemblies">List of assemblies</param>
    /// <returns></returns>
    public static IServiceCollection AddCliFxFromAssemblies(this IServiceCollection self, params Assembly[] assemblies)
    {
        IEnumerable<Type> commands = assemblies.SelectMany(t =>
            t.GetTypes().Where(s => s.IsAssignableTo(typeof(ICommand)) && s.IsInterface == false));
        self.Add(commands.Select(cmd => new ServiceDescriptor(cmd, cmd, ServiceLifetime.Transient)));
        return self;
    }
}
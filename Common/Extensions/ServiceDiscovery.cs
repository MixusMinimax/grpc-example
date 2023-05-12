using System.Reflection;
using Common.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class ServiceDiscovery
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection,
        params Assembly[] assemblies)
    {
        foreach (
            var (implementationType, annotation) in assemblies
                .SelectMany(a => a.DefinedTypes)
                .SelectMany(t => t.GetCustomAttributes<ServiceAttribute>()
                    .Select(a => (t, a)))
        )
        {
            var lifetime = annotation.ServiceLifetime;
            serviceCollection.Add(
                ServiceDescriptor.Describe(implementationType, implementationType, lifetime)
            );
            var interfaces = implementationType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                serviceCollection.Add(
                    ServiceDescriptor.Describe(@interface, implementationType, lifetime)
                );
            }
        }

        return serviceCollection;
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime ServiceLifetime { init; get; }
}
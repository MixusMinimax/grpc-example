using System.Linq.Expressions;
using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace UserService;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileWithDebugInfo();

        TypeAdapterConfig<Guid, string>.NewConfig().MapWith(src => src.ToString());

        TypeAdapterConfig<string, Guid>.NewConfig().MapWith(src => Guid.Parse(src));

        TypeAdapterConfig<Models.User, Proto.User.User>.NewConfig().IgnoreNullValues(true);

        TypeAdapterConfig<Models.HumanName, Proto.HumanName>.NewConfig().IgnoreNullValues(true);

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}
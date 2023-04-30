using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UserService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddDbContextPool<UserContext>(options =>
            options.UseLazyLoadingProxies().UseNpgsql(Configuration.GetConnectionString("PostgreSQL")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
                await context.Response.WriteAsJsonAsync(new
                {
                    results = endpointDataSource.Endpoints.OfType<RouteEndpoint>()
                        .Where(e => e.DisplayName?.StartsWith("gRPC") == true)
                        .Select(e => new
                        {
                            name = e.DisplayName, pattern = e.RoutePattern.RawText, order = e.Order
                        })
                        .ToList()
                });
            });

            // TODO: I do not want to use explicit implementation types here
            endpoints.MapGrpcService<Services.UserService>();
        });
    }
}
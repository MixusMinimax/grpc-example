using System.Reflection;
using Common.Attributes;
using Common.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
        services.AddServices(AppDomain.CurrentDomain.GetAssemblies());
        services.RegisterMapsterConfiguration();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudiences = Configuration.GetSection("JwtSettings:ValidAudiences").Get<string[]?>(),
                    IssuerSigningKey =
                        new JsonWebKey(File.ReadAllText(Configuration["JwtSettings:PublicKeyPath"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        services.AddAuthorization();
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
        app.UseAuthentication();
        app.UseAuthorization();

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

            foreach (var serviceType in AppDomain.CurrentDomain.GetAssemblies()
                         .Where(a => !a.IsDynamic)
                         .SelectMany(a => a.GetTypes())
                         .Where(t => t.GetCustomAttributes(typeof(GrpcServiceAttribute)).Count() != 0))
            {
                typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService")!.MakeGenericMethod(serviceType)
                    .Invoke(null, new object[] { endpoints });
            }
        });
    }
}
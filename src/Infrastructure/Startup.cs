using Infrastructure.Auth;
using Infrastructure.BackgroundJob;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddPersistence()
                .AddAuth(config)
                .AddBackgroundJob(config);
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseAuth();

            return app;
        }
    }
}

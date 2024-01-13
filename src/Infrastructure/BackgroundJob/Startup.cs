using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.BackgroundJob;

public static class Startup
{
    internal static IServiceCollection AddBackgroundJob(this IServiceCollection services, IConfiguration config)
    {
        var backgroundJobSetting = config.GetSection(nameof(BackgroundJobSetting)).Get<BackgroundJobSetting>();

        services.AddHostedService<InitializationDefaultStaffsBackgroundService>();

        if (backgroundJobSetting?.EnableJob is true) 
            services.AddHostedService<ProductStatisticsBackgroundService>();
        
        return services;
    }
}
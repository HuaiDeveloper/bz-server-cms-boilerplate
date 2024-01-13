using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Infrastructure.Persistence.EntityFrameworkCore;
using Infrastructure.Persistence.MongoDBDriver;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

internal static class Startup
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services
            .AddOptions<DatabaseSetting>()
            .BindConfiguration(nameof(DatabaseSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services
            .AddOptions<MongoDBSetting>()
            .BindConfiguration(nameof(MongoDBSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContextFactory<ApplicationDbContext>((p, b) =>
        {
            DatabaseSetting databaseSetting = p.GetRequiredService<IOptions<DatabaseSetting>>().Value;
            b.UseNpgsql(databaseSetting.ConnectionString, b => b.MigrationsAssembly("Infrastructure"));
        });

        services.AddScoped<MongoDBContext>();

        return services;
    }
}
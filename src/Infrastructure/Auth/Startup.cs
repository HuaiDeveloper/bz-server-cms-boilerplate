using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace Infrastructure.Auth;

public static class Startup
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = ".RebelBzApp.Cookies";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

        services.AddAuthorization(options =>
        {
            string[] authRoles = AuthRole.GetAuthRoles();
            foreach (var role in authRoles)
            {
                options.AddPolicy(role, (policy) => policy.RequireRole(role));
            }
        });

        services
            .AddScoped<CustomCookieAuthenticationEvents>()
            .AddScoped<StaffManager>();

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

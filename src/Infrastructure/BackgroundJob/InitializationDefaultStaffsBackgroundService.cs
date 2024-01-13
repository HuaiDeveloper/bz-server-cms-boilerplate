using Infrastructure.Auth;
using Infrastructure.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundJob;
public class InitializationDefaultStaffsBackgroundService : BackgroundService
{
    private readonly ILogger<InitializationDefaultStaffsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private DefaultStaff _defaultAdministrator =
        new DefaultStaff("admin", "admin@test.com", "123456", null, AuthRole.Admin);

    public InitializationDefaultStaffsBackgroundService(
        ILogger<InitializationDefaultStaffsBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await CreateDefaultStaffsAasync();
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(ProductStatisticsBackgroundService)} - while loop worker exception";
            _logger.LogError(exception: ex, message: errorMessage);
        }
    }

    private async Task CreateDefaultStaffsAasync()
    {
        using var scope = _serviceProvider.CreateScope();
        var staffManager = scope.ServiceProvider.GetRequiredService<StaffManager>();

        if (await staffManager.IsExistStaffNameAsync(_defaultAdministrator.Name) == false)
        {
            await staffManager.CreateAdminStaffAsync(
                _defaultAdministrator.Name,
                _defaultAdministrator.Email,
                _defaultAdministrator.Password,
                _defaultAdministrator.Description);

            var message = $"Create default administrator: {_defaultAdministrator.Name}";
            _logger.LogInformation(message);
        }
    }

    private record DefaultStaff(
        string Name,
        string Email,
        string Password,
        string? Description,
        string AuthRole);
}

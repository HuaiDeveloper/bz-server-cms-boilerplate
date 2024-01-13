using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Shared;
using Shared.Exceptions;

namespace Host.Components.Pages.Auth;

public partial class SignIn
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = null!;

    [SupplyParameterFromForm]
    private SignInRequest Request { get; set; } = new();

    [Inject]
    private StaffManager _staffManager { get; set; } = null!;

    [Inject]
    private NavigationManager _navigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task SubmitSignInAsync()
    {
        var isValidate = CustomValidator.TryValidateObject(Request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        var staff = await _staffManager.FindStaffAsync(Request.Name);
        var verifyPasswordResult = await _staffManager.VerifyPasswordAsync(staff, Request.Password);
        if (verifyPasswordResult == false)
            throw new NotFoundException("Verification failed");

        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.UserData, staff.Id.ToString()),
                    new Claim(ClaimTypes.Name, staff.Name),
                    new Claim(ClaimTypes.Role, staff.AuthRole),
                };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var validHours = 8;
        var issuedUtc = DateTimeOffset.UtcNow;
        var expiresUtc = issuedUtc.AddHours(validHours);

        var authProperties = new AuthenticationProperties()
        {
            IsPersistent = true,
            IssuedUtc = issuedUtc,
            ExpiresUtc = expiresUtc
        };

        if (HttpContext == null)
            throw new BaseException("httpContext is null");

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _navigationManager.NavigateTo("/", forceLoad: true);
    }

    private class SignInRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
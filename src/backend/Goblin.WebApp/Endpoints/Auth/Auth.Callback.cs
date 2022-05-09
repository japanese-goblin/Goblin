using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Goblin.WebApp.Endpoints.Auth;

public class AuthCallbackEndpoint : Endpoint<AuthEndpointRequest>
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthCallbackEndpoint(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("/auth/callback");
        RoutePrefixOverride("");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthEndpointRequest req, CancellationToken ct)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if(info == null)
        {
            AddError("Error loading external login information");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        if(result.Succeeded)
        {
            Logger.LogInformation("{0} logged in with {1} provider.", info.Principal.Identity.Name, info.LoginProvider);

            await SendRedirectAsync(req.ReturnUrl, cancellation: ct);
            return;
        }

        if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var user = new IdentityUser { UserName = email, Email = email };
            var createResult = await _userManager.CreateAsync(user);
            if(createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.User);
                var loginResult = await _userManager.AddLoginAsync(user, info);
                if(loginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    Logger.LogInformation("User created an account using {Provider} provider.", info.LoginProvider);
                }
            }
        }

        await SendOkAsync(ct); 
    }
}
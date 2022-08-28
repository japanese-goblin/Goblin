using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using VkNet.Utils;

namespace Goblin.WebApp.Endpoints.Auth;

public class AuthEndpoint : Endpoint<AuthEndpointRequest>
{
    private const string Provider = "Github";
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthEndpoint(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Get("/auth/");
        RoutePrefixOverride("");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthEndpointRequest req, CancellationToken ct)
    {
        var callbackUrl = "/auth/callback";
        if(!string.IsNullOrWhiteSpace(req.ReturnUrl))
        {
            callbackUrl += $"?returnUrl={req.ReturnUrl}";
        }
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(Provider, callbackUrl);
        HttpContext.Response.StatusCode = StatusCodes.Status302Found;
        HttpContext.Items[0] = null;
        await HttpContext.ChallengeAsync("github", properties);
    }
}

public class AuthEndpointRequest
{
    [QueryParam]
    public string ReturnUrl { get; set; }
}
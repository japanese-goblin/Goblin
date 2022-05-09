using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Goblin.WebApp.Endpoints.Auth;

public class AuthCheckEndpoint : Endpoint<AuthEndpointRequest>
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthCheckEndpoint(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    public override void Configure()
    {
        Get("/auth/check");
        RoutePrefixOverride("");
    }

    public override Task HandleAsync(AuthEndpointRequest req, CancellationToken ct)
    {
        return SendOkAsync(User.FindAll(ClaimTypes.Role).Select(x => x.Value), ct);
    }
}
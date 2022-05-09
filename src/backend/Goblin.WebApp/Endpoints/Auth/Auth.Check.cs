using FastEndpoints;

namespace Goblin.WebApp.Endpoints.Auth;

public class AuthCheckEndpoint : Endpoint<AuthEndpointRequest>
{
    public override void Configure()
    {
        Get("/auth/check");
        RoutePrefixOverride("");
    }

    public override Task HandleAsync(AuthEndpointRequest req, CancellationToken ct)
    {
        return SendOkAsync(ct);
    }
}
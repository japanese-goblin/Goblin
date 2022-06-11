using FastEndpoints;

namespace Goblin.WebApp.Endpoints.Admin.Users;

public class GetUsers : Endpoint<GetUsersRequest>
{
    public override void Configure()
    {
        Get("/admin/users");
    }

    public override Task HandleAsync(GetUsersRequest req, CancellationToken ct)
    {
        return SendOkAsync("auth", cancellation: ct);
    }
}

public class GetUsersRequest
{
    
}
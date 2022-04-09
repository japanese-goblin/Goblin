using Hangfire.Dashboard;

namespace Goblin.WebApp.Filters;

public class AuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return httpContext.User.IsInRole("Admin");
    }
}
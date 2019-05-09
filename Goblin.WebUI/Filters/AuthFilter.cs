using Hangfire.Dashboard;

namespace Goblin.WebUI.Filters
{
    public class AuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.IsInRole("Admin");
        }
    }
}

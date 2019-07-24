using Flurl.Http;

namespace Narfu
{
    public class RequestHelper
    {
        public static IFlurlRequest BuildRequest()
        {
            return Constants.EndPoint.WithTimeout(3)
                            .WithHeaders(new
                            {
                                User_Agent = Constants.UserAgent
                            })
                            .AllowAnyHttpStatus();
        }
    }
}
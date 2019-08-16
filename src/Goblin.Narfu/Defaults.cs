using Flurl.Http;

namespace Goblin.Narfu
{
    public class Defaults
    {
        public const string EndPoint = "https://ruz.narfu.ru/";

        public const string UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";

        internal static IFlurlRequest BuildRequest()
        {
            return EndPoint.WithTimeout(3)
                           .WithHeaders(new
                           {
                               User_Agent = UserAgent
                           });
        }
    }
}
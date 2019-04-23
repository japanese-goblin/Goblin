using System;
using System.Net.Http;

namespace Narfu
{
    internal static class Utils
    {
        internal static readonly HttpClient Client;
        internal const string EndPoint = "https://ruz.narfu.ru/";

        internal const string UserAgent =
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";

        static Utils()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(EndPoint),
                Timeout = TimeSpan.FromSeconds(3)
            };
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }
    }
}
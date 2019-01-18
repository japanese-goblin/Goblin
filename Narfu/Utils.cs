using System.Net;
using System.Net.Http;

namespace Narfu
{
    internal static class Utils
    {
        internal static readonly HttpClient Client;
        internal const string EndPoint = "https://ruz.narfu.ru";

        static Utils()
        {
            Client = new HttpClient();
        }
    }
}
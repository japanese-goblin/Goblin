using System.Net;
using HtmlAgilityPack;

namespace Narfu
{
    public static class Extensions
    {
        public static string GetNormalizedInnerText(this HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.InnerText
                                             .Trim()
                                             .Replace("\n", ""));
        }
    }
}
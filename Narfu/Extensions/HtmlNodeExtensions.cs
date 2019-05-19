using System.Net;
using HtmlAgilityPack;

namespace Narfu.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static string GetNormalizedInnerText(this HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.InnerText
                                             .Trim()
                                             .Replace("\n", ""));
        }
    }
}
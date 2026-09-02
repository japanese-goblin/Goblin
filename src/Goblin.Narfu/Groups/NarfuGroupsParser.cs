using System.Text.RegularExpressions;
using Goblin.Narfu.Abstractions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Group = Goblin.Narfu.Models.Group;

namespace Goblin.Narfu.Groups;

internal partial class NarfuGroupsParser(
    IHttpClientFactory httpClientFactory,
    ILogger<NarfuGroupsParser> logger) : INarfuGroupsParser
{
    public async Task<IReadOnlyCollection<Group>> ParseAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(Defaults.HttpClientName);
        var mainPage = await client.GetStringAsync("/", cancellationToken);
        var institutionIds = ParseInstitutionIds(mainPage);
        if (institutionIds.Count == 0)
        {
            throw new InvalidOperationException("На сайте САФУ не найдено ни одного института");
        }

        var groups = new List<Group>();
        foreach (var institutionId in institutionIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var groupsPage = await client.GetStringAsync($"/?groups&institution={institutionId}", cancellationToken);
            var institutionGroups = ParseGroups(groupsPage);
            groups.AddRange(institutionGroups);
            logger.LogDebug("Для института {InstitutionId} получено {GroupsCount} групп",
                institutionId,
                institutionGroups.Count);
        }

        var result = groups
            .DistinctBy(group => group.RealId)
            .OrderBy(group => group.RealId)
            .ToArray();
        if (result.Length == 0)
        {
            throw new InvalidOperationException("Сайт САФУ вернул пустой список групп");
        }

        return result;
    }

    private static IReadOnlyCollection<int> ParseInstitutionIds(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return (document.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            .Select(node => TryGetQueryInt(node.GetAttributeValue("href", string.Empty), "institution"))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();
    }

    private static IReadOnlyCollection<Group> ParseGroups(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var groups = new List<Group>();
        foreach (var link in document.DocumentNode.SelectNodes(
                                 "//a[@href][.//span[contains(concat(' ', normalize-space(@class), ' '), ' number ')]]") ??
                             Enumerable.Empty<HtmlNode>())
        {
            var siteId = TryGetQueryInt(link.GetAttributeValue("href", string.Empty), "group");
            var numberNode = link.SelectSingleNode(".//span[contains(concat(' ', normalize-space(@class), ' '), ' number ')]");
            if (siteId is null || numberNode is null || !int.TryParse(numberNode.InnerText.Trim(), out var realId))
            {
                continue;
            }

            var fullText = NormalizeWhitespaceRegex().Replace(HtmlEntity.DeEntitize(link.InnerText), " ").Trim();
            var realIdText = realId.ToString();
            var groupName = fullText.StartsWith(realIdText, StringComparison.Ordinal)
                ? fullText[realIdText.Length..].Trim()
                : fullText;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                continue;
            }

            groups.Add(new Group(groupName, realId, siteId.Value));
        }

        return groups
            .DistinctBy(group => group.RealId)
            .ToArray();
    }

    private static int? TryGetQueryInt(string link, string parameterName)
    {
        var queryStart = link.IndexOf('?');
        if (queryStart < 0 || queryStart == link.Length - 1)
        {
            return null;
        }

        foreach (var part in link[(queryStart + 1)..].Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = part.Split('=', 2);
            if (pair.Length == 2 &&
                pair[0].Equals(parameterName, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(pair[1], out var value))
            {
                return value;
            }
        }

        return null;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeWhitespaceRegex();
}

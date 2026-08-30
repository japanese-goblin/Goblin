using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Goblin.Application.Core.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetAsync<T>(this IDistributedCache distributedCache, 
                                             string key,
                                             CancellationToken ct = default)
            where T : notnull
    {
        var cachedData = await distributedCache.GetAsync(key, ct);
        if(cachedData is null)
        {
            return default;
        }

        var value = JsonSerializer.Deserialize<T?>(cachedData);
        return value;
    }

    public static async Task SetAsync<T>(this IDistributedCache distributedCache,
                                         string key, 
                                         T? value,
                                         DistributedCacheEntryOptions options,
                                         CancellationToken ct = default)
            where T : notnull
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(value);
        await distributedCache.SetAsync(key, data, options, ct);
    }
}
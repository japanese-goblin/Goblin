using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Goblin.Application.Core.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetAsync<T>(this IDistributedCache distributedCache, 
                                             string key,
                                             CancellationToken token = default)
            where T : notnull, new()
    {
        var cachedData = await distributedCache.GetAsync(key, token);
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
                                         DistributedCacheEntryOptions options)
            where T : notnull, new()
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(value);
        await distributedCache.SetAsync(key, data, options);
    }
}
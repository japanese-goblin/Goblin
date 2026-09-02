using System.Text.Json;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Goblin.Narfu.Groups;

internal sealed class RedisNarfuGroupsCache(
    IDistributedCache cache,
    ILogger<RedisNarfuGroupsCache> logger) : INarfuGroupsCache
{
    private const string CacheKey = "narfu:groups:list";
    private static readonly TimeSpan LocalSnapshotLifetime = TimeSpan.FromMinutes(1);

    private readonly Lock _snapshotLock = new Lock();
    private IReadOnlyDictionary<int, Group>? _snapshot;
    private DateTimeOffset _snapshotLoadedAt;

    public Group? GetByRealId(int realGroupId)
    {
        var groups = GetSnapshot();
        return groups.GetValueOrDefault(realGroupId);
    }

    public async Task ReplaceAsync(IReadOnlyCollection<Group> groups, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(groups);
        if (groups.Count == 0)
        {
            throw new ArgumentException("Список групп не может быть пустым", nameof(groups));
        }

        var normalizedGroups = groups
            .DistinctBy(group => group.RealId)
            .OrderBy(group => group.RealId)
            .ToArray();
        var serializedGroups = JsonSerializer.SerializeToUtf8Bytes(normalizedGroups);

        await cache.SetAsync(CacheKey, serializedGroups, cancellationToken);
        SetSnapshot(normalizedGroups);
    }

    private IReadOnlyDictionary<int, Group> GetSnapshot()
    {
        lock(_snapshotLock)
        {
            if (_snapshot is not null && DateTimeOffset.UtcNow - _snapshotLoadedAt < LocalSnapshotLifetime)
            {
                return _snapshot;
            }

            try
            {
                var serializedGroups = cache.Get(CacheKey);
                if (serializedGroups is null)
                {
                    TouchSnapshot();
                    return _snapshot!;
                }

                var groups = JsonSerializer.Deserialize<Group[]>(serializedGroups) ?? [];
                if (groups.Length == 0)
                {
                    logger.LogWarning("В Redis найден пустой список групп САФУ; используется предыдущий локальный снимок");
                    TouchSnapshot();
                    return _snapshot!;
                }

                SetSnapshotUnsafe(groups);
            }
            catch(Exception exception)
            {
                logger.LogError(exception, "Не удалось получить список групп САФУ из Redis");
                TouchSnapshot();
            }

            return _snapshot!;
        }
    }

    private void SetSnapshot(IReadOnlyCollection<Group> groups)
    {
        lock(_snapshotLock)
        {
            SetSnapshotUnsafe(groups);
        }
    }

    private void SetSnapshotUnsafe(IEnumerable<Group> groups)
    {
        _snapshot = groups.ToDictionary(group => group.RealId);
        _snapshotLoadedAt = DateTimeOffset.UtcNow;
    }

    private void TouchSnapshot()
    {
        _snapshot ??= new Dictionary<int, Group>();
        _snapshotLoadedAt = DateTimeOffset.UtcNow;
    }
}

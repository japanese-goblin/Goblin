using Goblin.Narfu.Models;

namespace Goblin.Narfu.Abstractions;

public interface INarfuGroupsCache
{
    Group? GetByRealId(int realGroupId);

    Task ReplaceAsync(IReadOnlyCollection<Group> groups, CancellationToken cancellationToken);
}

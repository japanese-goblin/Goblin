using Goblin.Narfu.Models;

namespace Goblin.Narfu.Abstractions;

public interface INarfuGroupsParser
{
    Task<IReadOnlyCollection<Group>> ParseAsync(CancellationToken cancellationToken);
}

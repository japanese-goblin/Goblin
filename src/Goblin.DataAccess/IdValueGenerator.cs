using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Goblin.DataAccess;

internal class IdValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
    {
        return UUIDNext.Uuid.NewSequential();
    }

    public override bool GeneratesTemporaryValues { get; } = false;
}

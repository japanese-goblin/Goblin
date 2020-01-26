using System.Collections.Generic;
using System.Linq;

namespace Goblin.Application.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
        {
            while(source.Any())
            {
                yield return source.Take(size);
                source = source.Skip(size);
            }
        }
    }
}
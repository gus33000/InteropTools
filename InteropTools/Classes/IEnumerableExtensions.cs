using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteropTools.Classes
{
    public static class IEnumerableExtensions
    {
        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await funcBody(partition.Current); }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }
    }
}

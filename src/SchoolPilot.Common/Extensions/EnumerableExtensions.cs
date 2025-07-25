

namespace SchoolPilot.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashset<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static void AddIfNotEmpty(this HashSet<Guid> source, Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                source.Add(id.Value);
            }
        }

        /// <summary>
        /// Converts a List to a HashSet:
        /// It takes a list(IEnumerable<TSource>) and converts it into a HashSet<TResult>.
        /// A HashSet is a collection that does not allow duplicate values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return new HashSet<TResult>(source.Select(selector));
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Any();
        }

        public static IEnumerable<IEnumerable<T>> GetBatches<T>(this IReadOnlyCollection<T> claimIds, int batchSize = 100)
        {
            for (var index = 0; index < claimIds.Count; index += batchSize)
            {
                yield return claimIds.Skip(index).Take(batchSize);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace VeniceDomain.Extensions
{
    public static class IEnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
                action(element);
        }

        public static IEnumerable<TResult> ConvertAll<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> func)
        {
            foreach (var element in enumerable)
                yield return func(element);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

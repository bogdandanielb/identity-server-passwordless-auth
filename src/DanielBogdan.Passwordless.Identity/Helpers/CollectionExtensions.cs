using System.Collections.Generic;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class CollectionExtensions
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> range)
        {
            if (range == null || range.Count == 0) return;

            foreach (var keyValuePair in range)
            {
                dic.TryAdd(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public static void AddRange<T>(this ICollection<T> list, ICollection<T> items)
        {
            if (items == null || items.Count == 0) return;

            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}

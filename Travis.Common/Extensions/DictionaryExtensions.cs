using System.Collections.Generic;
using System.Linq;

namespace Travis.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Makes shallow copy of dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}

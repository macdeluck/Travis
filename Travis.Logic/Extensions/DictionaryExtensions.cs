using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Makes shallow copy of dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return new Dictionary<TKey, TValue>(source);
        }
    }
}

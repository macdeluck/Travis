using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Extensions
{
    /// <summary>
    /// Contains additional methods for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Makes shallow copy of dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return new Dictionary<TKey, TValue>(source);
        }

        /// <summary>
        /// Compares two dictionaries using default value <see cref="EqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of dictionary value.</typeparam>
        /// <param name="first">First dictionary.</param>
        /// <param name="second">Second dictionary.</param>
        /// <returns>True, if dictionary elements are equal.</returns>
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            return first.DictionaryEquals(second, null);
        }

        /// <summary>
        /// Compares two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key.</typeparam>
        /// <typeparam name="TValue">Type of dictionary value.</typeparam>
        /// <param name="first">First dictionary.</param>
        /// <param name="second">Second dictionary.</param>
        /// <param name="valueComparer">Comparer of values.</param>
        /// <returns>True, if dictionary elements are equal.</returns>
        public static bool DictionaryEquals<TKey, TValue>(
            this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second,
            IEqualityComparer<TValue> valueComparer)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!valueComparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }
}

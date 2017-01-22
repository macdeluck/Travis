using System;
using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Extensions
{
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Chooses random element from collection.
        /// </summary>
        /// <typeparam name="T">Type of collection element.</typeparam>
        /// <returns>Random element from collection.</returns>
        public static T RandomElement<T>(this IEnumerable<T> collection)
        {
            return collection.RandomElement(new Random());
        }

        /// <summary>
        /// Chooses random element from collection.
        /// </summary>
        /// <typeparam name="T">Type of collection element.</typeparam>
        /// <param name="random">Random object used to select element.</param>
        /// <returns>Random element from collection.</returns>
        public static T RandomElement<T>(this IEnumerable<T> collection, Random random)
        {
            var num = collection.Count();
            if (num <= 0)
                throw new ArgumentException(Messages.CollectionCannotBeEmpty);
            return collection.ElementAt(random.Next(num));
        }

        /// <summary>
        /// Permutes collection.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to permute.</param>
        /// <returns>Collection containing the same elements but in different order.</returns>
        public static IList<T> Permute<T>(this IEnumerable<T> collection)
        {
            var result = new List<T>(collection);
            var random = new Random();
            for (int i = result.Count - 1; i >= 0; i--)
            {
                var index = random.Next(i);
                var tmp = result[index];
                result[index] = result[i];
                result[i] = result[index];
            }
            return result;
        }

        #region ARGMAX

        private class TComparer<T> : IComparer<T>
            where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return x.CompareTo(y);
            }
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <typeparam name="TVal">Type of value to select.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        /// <param name="comparer">Comparer of values.</param>
        /// <param name="minVal">Minimum value of <typeparamref name="TVal"/> type.</param>
        public static List<T> ArgMax<T, TVal>(this IEnumerable<T> collection, Func<T, TVal> selector, IComparer<TVal> comparer, TVal minVal)
        {
            var max = minVal;
            var maxItems = new List<T>();
            foreach (var item in collection)
            {
                var value = selector(item);
                var compResult = comparer.Compare(value, max);
                if (compResult == 0)
                {
                    maxItems.Add(item);
                }
                else if (compResult > 0)
                {
                    max = value;
                    maxItems.Clear();
                    maxItems.Add(item);
                }
            }
            return maxItems;
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <typeparam name="TVal">Type of value to select.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        /// <param name="minVal">Minimum value of <typeparamref name="TVal"/> type.</param>
        public static List<T> ArgMax<T, TVal>(this IEnumerable<T> collection, Func<T, TVal> selector, TVal minVal)
            where TVal : IComparable<TVal>
        {
            return collection.ArgMax(selector, new TComparer<TVal>(), minVal);
        }
        
        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, byte> selector)
        {
            return collection.ArgMax(selector, byte.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, sbyte> selector)
        {
            return collection.ArgMax(selector, sbyte.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, short> selector)
        {
            return collection.ArgMax(selector, short.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, ushort> selector)
        {
            return collection.ArgMax(selector, ushort.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            return collection.ArgMax(selector, int.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, uint> selector)
        {
            return collection.ArgMax(selector, uint.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, long> selector)
        {
            return collection.ArgMax(selector, long.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, ulong> selector)
        {
            return collection.ArgMax(selector, ulong.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, float> selector)
        {
            return collection.ArgMax(selector, float.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, double> selector)
        {
            return collection.ArgMax(selector, double.MinValue);
        }

        /// <summary>
        /// Returns elements for whose <paramref name="selector"/> returned maximum value.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="collection">Collection to traverse.</param>
        /// <param name="selector">Selector of values.</param>
        public static List<T> ArgMax<T>(this IEnumerable<T> collection, Func<T, decimal> selector)
        {
            return collection.ArgMax(selector, decimal.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<byte> ArgMax(this IEnumerable<byte> collection)
        {
            return collection.ArgMax(v => v, byte.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<sbyte> ArgMax(this IEnumerable<sbyte> collection)
        {
            return collection.ArgMax(v => v, sbyte.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<short> ArgMax(this IEnumerable<short> collection)
        {
            return collection.ArgMax(v => v, short.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<ushort> ArgMax(this IEnumerable<ushort> collection)
        {
            return collection.ArgMax(v => v, ushort.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<int> ArgMax(this IEnumerable<int> collection)
        {
            return collection.ArgMax(v => v, int.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<uint> ArgMax(this IEnumerable<uint> collection)
        {
            return collection.ArgMax(v => v, uint.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<long> ArgMax(this IEnumerable<long> collection)
        {
            return collection.ArgMax(v => v, long.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<ulong> ArgMax(this IEnumerable<ulong> collection)
        {
            return collection.ArgMax(v => v, ulong.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<float> ArgMax(this IEnumerable<float> collection)
        {
            return collection.ArgMax(v => v, float.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<double> ArgMax(this IEnumerable<double> collection)
        {
            return collection.ArgMax(v => v, double.MinValue);
        }

        /// <summary>
        /// Returns elements with maximum value.
        /// </summary>
        /// <param name="collection">Collection to traverse.</param>
        public static List<decimal> ArgMax(this IEnumerable<decimal> collection)
        {
            return collection.ArgMax(v => v, decimal.MinValue);
        }
        #endregion
    }
}

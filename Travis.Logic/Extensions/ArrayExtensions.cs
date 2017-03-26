using System;
using System.Linq;

namespace Travis.Logic.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Array"/>s.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Performs copy of two-dimentional array.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to clone.</param>
        /// <param name="cloneFunction">Function used to clone array elements.</param>
        public static T[,] CloneArray<T>(this T[,] array, Func<T, T> cloneFunction)
        {
            int n = array.GetLength(0);
            int m = array.GetLength(1);
            var result = new T[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[i, j] = cloneFunction(array[i, j]);
            return result;
        }

        /// <summary>
        /// Performs copy of two-dimentional array.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to clone.</param>
        public static T[,] CloneArray<T>(this T[,] array)
        {
            return array.CloneArray(t => t);
        }

        /// <summary>
        /// Performs copy of one-dimentional array.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to clone.</param>
        /// <param name="cloneFunction">Function used to clone array elements.</param>
        public static T[] CloneArray<T>(this T[] array, Func<T, T> cloneFunction)
        {
            int n = array.Length;
            var result = new T[n];
            for (int i = 0; i < n; i++)
                result[i] = cloneFunction(array[i]);
            return result;
        }

        /// <summary>
        /// Performs copy of one-dimentional array.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to clone.</param>
        public static T[] CloneArray<T>(this T[] array)
        {
            return array.CloneArray(t => t);
        }

        /// <summary>
        /// Fills array with given element.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to fill.</param>
        /// <param name="element">Element to put in array.</param>
        public static T[,] Fill<T>(this T[,] array, T element)
        {
            int n = array.GetLength(0);
            int m = array.GetLength(1);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    array[i, j] = element;
            return array;
        }

        /// <summary>
        /// Transposes array.
        /// </summary>
        /// <typeparam name="T">Type of array element.</typeparam>
        /// <param name="array">Array to transpose.</param>
        public static T[,] Transpose<T>(this T[,] array)
        {
            var result = new T[array.GetLength(1), array.GetLength(0)];
            for (int x = 0; x < array.GetLength(0); x++)
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    result[y, x] = array[x, y];
                }
            return result;
        }
    }
}

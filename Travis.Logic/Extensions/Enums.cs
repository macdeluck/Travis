using System;
using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Extensions
{
    /// <summary>
    /// Extensions for enumerable types.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Enumerates declared enum values.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        public static IEnumerable<T> Values<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"T must be an {typeof(Enum)} type instance");
            }
            var arr = Enum.GetValues(typeof(T));
            return arr.OfType<T>();
        }
    }
}

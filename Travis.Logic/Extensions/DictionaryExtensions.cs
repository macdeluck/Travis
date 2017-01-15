﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Extensions
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

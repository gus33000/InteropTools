// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Intense
{
    /// <summary>
    /// Provides extension helpers for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns distinct elements from the collection by using specified key selector.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            HashSet<TKey> keys = new();
            foreach (T element in source)
            {
                if (keys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
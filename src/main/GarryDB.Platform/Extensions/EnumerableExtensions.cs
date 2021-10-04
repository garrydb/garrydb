using System;
using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Execute <paramref name="action" /> on each item in <paramref name="enumerable" />.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}" />.</param>
        /// <param name="action">The actino to execute.</param>
        /// <typeparam name="T">The type of the items in <paramref name="enumerable" />.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Concatenates <paramref name="item" /> to the <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}" /> to concatenate the <paramref name="item" /> to.</param>
        /// <param name="item">The item to concatenate.</param>
        /// <typeparam name="T">The type of the <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> where <paramref name="item" /> is added,
        ///     unless <paramref name="item" /> is <c>null</c>.
        /// </returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T? item)
        {
            return enumerable.Concat(item.AsEnumerable());
        }

        /// <summary>
        ///     Produces an <see cref="IEnumerable{T}" /> without <paramref name="item" />.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}" /> to exclude the <paramref name="item" /> from.</param>
        /// <param name="item">The item to exclude.</param>
        /// <typeparam name="T">The type of the <see cref="IEnumerable{T}" />.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> where <paramref name="item" /> is excluded</returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T? item)
        {
            return enumerable.Except(item.AsEnumerable());
        }
    }
}

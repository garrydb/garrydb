using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="object" />.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        ///     Returns an <see cref="IEnumerable{T}" /> containing <paramref name="item" />.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> containing <paramref name="item" />,
        ///     or an empty <see cref="IEnumerable{T}" /> if <paramref name="item" /> is <c>null</c>.
        /// </returns>
        public static IEnumerable<T> AsEnumerable<T>(this T? item)
        {
            if (item == null)
            {
                return Enumerable.Empty<T>();
            }

            return new[]
                   {
                       item
                   };
        }
    }
}

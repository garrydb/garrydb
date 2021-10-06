using System.Globalization;

namespace GarryDb.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="string" />.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix, bool ignoreCase)
        {
            if (s.EndsWith(suffix, ignoreCase, CultureInfo.CurrentCulture))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }
    }
}

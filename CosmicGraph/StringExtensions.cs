using System.IO;
using System.Text.RegularExpressions;

namespace CosmicGraph
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string that)
        {
            if (that == null)
            {
                return null;
            }

            if (that.Length == 0)
            {
                return that;
            }

            if (that.Length == 1)
            {
                return char.ToLowerInvariant(that[0]).ToString();
            }

            return char.ToLowerInvariant(that[0]) + that.Substring(1);
        }

        public static string AddEscapeCharacters(this string that)
        {
            if (that == null)
            {
                return that;
            }

            return that.Replace("'", "\\'");
        }
    }
}

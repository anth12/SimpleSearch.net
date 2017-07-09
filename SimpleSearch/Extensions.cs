using System.Text.RegularExpressions;

namespace SimpleSearch
{
    internal static class Extensions
    {
        private static Regex CleanRegex = new Regex(@"[^\w]");

        internal static string Clean(this string source)
        {
            return CleanRegex.Replace(source, "").ToLower();
        }
    }
}

using System;

namespace Discord
{
    internal static class StringExtensions
    {
        public static bool IsNullOrUri(this string url) =>
            string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
}

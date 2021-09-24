using System;

namespace Discord.Utils
{
    static class UrlValidation
    {
        /// <summary>
        /// Not full URL validation right now. Just ensures protocol is present and that it's either http or https
        /// </summary>
        /// <param name="url">url to validate before sending to Discord.</param>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
        /// <returns>true if url is valid by our standard, false if null, throws an error upon invalid </returns>
        public static bool Validate(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if(!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException($"Url {url} must be include its protocol (either HTTP or HTTPS)");
            return true;
        }
    }
}

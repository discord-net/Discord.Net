using System;

namespace Discord.Utils
{
    static class UrlValidation
    {
        /// <summary>
        /// Not full URL validation right now. Just ensures protocol is present and that it's either http or https
        /// <see cref="ValidateButton(string)"/> should be used for url buttons
        /// </summary>
        /// <param name="url">url to validate before sending to Discord.</param>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
        /// <returns>true if url is valid by our standard, false if null, throws an error upon invalid </returns>
        public static bool Validate(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if(!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException($"The url {url} must include a protocol (either HTTP or HTTPS)");
            return true;
        }

        /// <summary>
        /// Not full URL validation right now. Just Ensures the protocol is either http, https, or discord
        /// <see cref="Validate(string)"/> should be used everything other than url buttons
        /// </summary>
        /// <param name="url">the url to validate before sending to discord</param>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (either http, https, or discord).</exception>
        /// <returns>true if the url is valid by our standard, false if null, throws an error upon invalid</returns>
        public static bool ValidateButton(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if(!((url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) || 
                (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) || 
                (url.StartsWith("discord://", StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException($"The url {url} must include a protocol (either HTTP, HTTPS, or DISCORD)");
            return true;
        }
    }
}

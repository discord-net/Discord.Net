using System;

namespace Discord.Utils
{
    internal static class UrlValidation
    {
        /// <summary>
        ///     Not full URL validation right now. Just ensures protocol is present and that it's either http or https
        /// <see cref="ValidateButton(string)"/> should be used for url buttons.
        /// </summary>
        /// <param name="url">The URL to validate before sending to Discord.</param>
        /// <param name="allowAttachments"><see langword="true"/> to allow the <b>attachment://</b> protocol; otherwise <see langword="false"/>.</param>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
        /// <returns>true if URL is valid by our standard, false if null, throws an error upon invalid.</returns>
        public static bool Validate(string url, bool allowAttachments = false)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || (allowAttachments ? url.StartsWith("attachment://", StringComparison.Ordinal) : false)))
                throw new InvalidOperationException($"The url {url} must include a protocol (either {(allowAttachments ? "HTTP, HTTPS, or ATTACHMENT" : "HTTP or HTTPS")})");
            return true;
        }

        /// <summary>
        ///     Not full URL validation right now. Just Ensures the protocol is either http, https, or discord
        /// <see cref="Validate(string, bool)"/> should be used everything other than url buttons.
        /// </summary>
        /// <param name="url">The URL to validate before sending to discord.</param>
        /// <exception cref="InvalidOperationException">A URL must include a protocol (either http, https, or discord).</exception>
        /// <returns>true if the URL is valid by our standard, false if null, throws an error upon invalid.</returns>
        public static bool ValidateButton(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("discord://", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"The url {url} must include a protocol (either HTTP, HTTPS, or DISCORD)");
            return true;
        }
    }
}

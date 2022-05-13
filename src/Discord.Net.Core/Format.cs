using System.Text;
using System.Text.RegularExpressions;

namespace Discord
{
    /// <summary> A helper class for formatting characters. </summary>
    public static class Format
    {
        // Characters which need escaping
        private static readonly string[] SensitiveCharacters = {
            "\\", "*", "_", "~", "`", ".", ":", "/", ">", "|" };

        /// <summary> Returns a markdown-formatted string with bold formatting. </summary>
        public static string Bold(string text) => $"**{text}**";
        /// <summary> Returns a markdown-formatted string with italics formatting. </summary>
        public static string Italics(string text) => $"*{text}*";
        /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
        public static string Underline(string text) => $"__{text}__";
        /// <summary> Returns a markdown-formatted string with strike-through formatting. </summary>
        public static string Strikethrough(string text) => $"~~{text}~~";
        /// <summary> Returns a string with spoiler formatting. </summary>
        public static string Spoiler(string text) => $"||{text}||";
        /// <summary> Returns a markdown-formatted URL. Only works in <see cref="EmbedBuilder"/> descriptions and fields. </summary>
        public static string Url(string text, string url) => $"[{text}]({url})";
        /// <summary> Escapes a URL so that a preview is not generated. </summary>
        public static string EscapeUrl(string url) => $"<{url}>";

        /// <summary> Returns a markdown-formatted string with codeblock formatting. </summary>
        public static string Code(string text, string language = null)
        {
            if (language != null || text.Contains("\n"))
                return $"```{language ?? ""}\n{text}\n```";
            else
                return $"`{text}`";
        }

        /// <summary> Sanitizes the string, safely escaping any Markdown sequences. </summary>
        public static string Sanitize(string text)
        {
            if (text != null)
                foreach (string unsafeChar in SensitiveCharacters)
                    text = text.Replace(unsafeChar, $"\\{unsafeChar}");
            return text;
        }

        /// <summary>
        ///     Formats a string as a quote.
        /// </summary>
        /// <param name="text">The text to format.</param>
        /// <returns>Gets the formatted quote text.</returns>
        public static string Quote(string text)
        {
            // do not modify null or whitespace text
            // whitespace does not get quoted properly
            if (string.IsNullOrWhiteSpace(text))
                return text;

            StringBuilder result = new StringBuilder();

            int startIndex = 0;
            int newLineIndex;
            do
            {
                newLineIndex = text.IndexOf('\n', startIndex);
                if (newLineIndex == -1)
                {
                    // read the rest of the string
                    var str = text.Substring(startIndex);
                    result.Append($"> {str}");
                }
                else
                {
                    // read until the next newline
                    var str = text.Substring(startIndex, newLineIndex - startIndex);
                    result.Append($"> {str}\n");
                }
                startIndex = newLineIndex + 1;
            }
            while (newLineIndex != -1 && startIndex != text.Length);

            return result.ToString();
        }
        
        /// <summary>
        ///     Formats a string as a block quote.
        /// </summary>
        /// <param name="text">The text to format.</param>
        /// <returns>Gets the formatted block quote text.</returns>
        public static string BlockQuote(string text)
        {
            // do not modify null or whitespace
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return $">>> {text}";
        }

        /// <summary>
        /// Remove discord supported markdown from text.
        /// </summary>
        /// <param name="text">The to remove markdown from.</param>
        /// <returns>Gets the unformatted text.</returns>
        public static string StripMarkDown(string text)
        {
            //Remove discord supported markdown
            var newText = Regex.Replace(text, @"(\*|_|`|~|>|\\)", "");
            return newText;
        }

        /// <summary>
        ///     Formats a user's username + discriminator.
        /// </summary>
        /// <param name="doBidirectional">To format the string in bidirectional unicode or not</param>
        /// <param name="user">The user whos username and discriminator to format</param>
        /// <returns>The username + discriminator</returns>
        public static string UsernameAndDiscriminator(IUser user, bool doBidirectional)
        {
            return doBidirectional
                ? $"\u2066{user.Username}\u2069#{user.Discriminator}"
                : $"{user.Username}#{user.Discriminator}";
        }
    }
}

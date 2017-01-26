namespace Discord
{
    public static class Format
    {
        // Characters which need escaping
        private static string[] SensitiveCharacters = { "\\", "*", "_", "~", "`" };

        /// <summary> Returns a markdown-formatted string with bold formatting. </summary>
        public static string Bold(string text) => $"**{text}**";
        /// <summary> Returns a markdown-formatted string with italics formatting. </summary>
        public static string Italics(string text) => $"*{text}*";
        /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
        public static string Underline(string text) => $"__{text}__";
        /// <summary> Returns a markdown-formatted string with strikethrough formatting. </summary>
        public static string Strikethrough(string text) => $"~~{text}~~";

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
            foreach (string unsafeChar in SensitiveCharacters)
                text = text.Replace(unsafeChar, $"\\{unsafeChar}");
            return text;
        }
    }
}

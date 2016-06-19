namespace Discord
{
    public static class Format
    {
        /// <summary> Returns a markdown-formatted string with bold formatting. </summary>
        public static string Bold(string text) => $"**{text}**";
        /// <summary> Returns a markdown-formatted string with italics formatting. </summary>
        public static string Italics(string text) => $"*{text}*";
        /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
        public static string Underline(string text) => $"__{text}__";
        /// <summary> Returns a markdown-formatted string with strikethrough formatting. </summary>
        public static string Strikethrough(string text) => $"~~{text}~~";

        /// <summary> Returns a markdown-formatted string with strikeout formatting. </summary>
        public static string Code(string text, string language = null)
        {
            if (language != null || text.Contains("\n"))
                return $"```{language ?? ""}\n{text}\n```";
            else
                return $"`{text}`";
        }
    }
}

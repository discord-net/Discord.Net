using System.Text;
using System.Text.RegularExpressions;

namespace Discord
{
    public static class Format
	{
		private static readonly Regex _escapeRegex;
		private static readonly MatchEvaluator _escapeEvaluator;

		static Format()
		{
			const string innerPattern = "[_*]|~~";
			_escapeRegex = new Regex($@"(?<=^|\W)(?:{innerPattern})|(?:{innerPattern})(?=\W|$)|\\", RegexOptions.Compiled);
			_escapeEvaluator = new MatchEvaluator(e => '\\' + e.Value);
		}

		/// <summary> Removes all special formatting characters from the provided text. </summary>
		private static string Escape(string text)
			=> _escapeRegex.Replace(text, _escapeEvaluator);

		/// <summary> Returns a markdown-formatted string with no formatting, optionally escaping the contents. </summary>
		public static string Normal(string text, bool escape = true)
			=> escape ? Escape(text) : text;
		/// <summary> Returns a markdown-formatted string with bold formatting, optionally escaping the contents. </summary>
		public static string Bold(string text, bool escape = true)
			=> escape ? $"**{Escape(text)}**" : $"**{text}**";
		/// <summary> Returns a markdown-formatted string with italics formatting, optionally escaping the contents. </summary>
		public static string Italics(string text, bool escape = true)
			=> escape ? $"*{Escape(text)}*" : $"*{text}*";
		/// <summary> Returns a markdown-formatted string with underline formatting, optionally escaping the contents. </summary>
		public static string Underline(string text, bool escape = true)
			=> escape ? $"__{Escape(text)}__" : $"__{text}__";
		/// <summary> Returns a markdown-formatted string with strikeout formatting, optionally escaping the contents. </summary>
		public static string Strikeout(string text, bool escape = true)
			=> escape ? $"~~{Escape(text)}~~" : $"~~{text}~~";

		/// <summary> Returns a markdown-formatted string with multiple formatting, optionally escaping the contents. </summary>
		public static string Multiple(string text, bool escape = true, bool bold = false, bool italics = false, bool underline = false, bool strikeout = false)
		{
			string result = text;
			if (escape) result = Escape(result);
			if (bold) result = Bold(result, false);
			if (italics) result = Italics(result, false);
			if (underline) result = Underline(result, false);
			if (strikeout) result = Strikeout(result, false);
			return result;
		}
	}
}

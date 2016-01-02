using System.Text;

namespace Discord
{
	public static class Format
	{
		private static readonly string[] _patterns;
		private static readonly StringBuilder _builder;

        static Format()
		{
			_patterns = new string[] { "__", "_", "**", "*", "~~", "```", "`"};
			_builder = new StringBuilder(DiscordConfig.MaxMessageSize);
        }

		/// <summary> Removes all special formatting characters from the provided text. </summary>
		public static string Escape(string text)
		{
			lock (_builder)
			{
				_builder.Clear();

				//Escape all backslashes
				for (int i = 0; i < text.Length; i++)
				{
					_builder.Append(text[i]);
					if (text[i] == '\\')
						_builder.Append('\\');
				}

				EscapeSubstring(0, _builder.Length);

				return _builder.ToString();
			}
		}
		private static int EscapeSubstring(int start, int end)
		{
			int totalAddedChars = 0;
			for (int i = start; i < end + totalAddedChars; i++)
			{
				for (int p = 0; p < _patterns.Length; p++)
				{
					string pattern = _patterns[p];
					if (i + pattern.Length * 2 > _builder.Length)
						continue;
					int s = FindPattern(pattern, i, i + 1);
					if (s == -1) continue;
					int e = FindPattern(pattern, i + 1, end + totalAddedChars);
					if (e == -1) continue;

					if (e - s - pattern.Length > 0)
					{
						//By going right to left, we dont need to adjust any offsets
						for (int k = pattern.Length - 1; k >= 0; k--)
							_builder.Insert(e + k, '\\');
						for (int k = pattern.Length - 1; k >= 0; k--)
							_builder.Insert(s + k, '\\');
                        int addedChars = pattern.Length * 2;
						addedChars += EscapeSubstring(s + pattern.Length * 2, e + pattern.Length);
						i = e + addedChars + pattern.Length - 1;
						totalAddedChars += addedChars;
						break;
                    }
				}
			}
			return totalAddedChars;
		}
		private static int FindPattern(string pattern, int start, int end)
		{
			for (int j = start; j < end; j++)
			{
				if (_builder[j] == '\\')
				{
					j++;
					continue;
				}
				for (int k = 0; k < pattern.Length; k++)
				{
					if (_builder[j + k] != pattern[k])
						goto nextpos;
				}
				return j;
				nextpos:;
			}
			return -1;
		}
        
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

		/// <summary> Returns a markdown-formatted string with strikeout formatting, optionally escaping the contents. </summary>
		public static string Code(string text, string language = null)
		{
			if (language != null || text.Contains("\n"))
				return $"```{language ?? ""}\n{text}\n```";
			else
				return $"`{text}`";
		}
	}
}

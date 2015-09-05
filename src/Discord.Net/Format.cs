using System.Text;

namespace Discord
{
    public static class Format
	{
		private static char[] specialChars = new char[] {'_', '*', '~', '\\' }; //Backslash must always be last!

		/// <summary> Removes all special formatting characters from the provided text. </summary>
		private static string Escape(string text)
		{
			if (text.IndexOfAny(specialChars) >= 0)
			{
				StringBuilder builder = new StringBuilder(text);
				foreach (char c in specialChars)
				{
					int length = builder.Length;
					for (int i = 0; i < length; i++)
					{
						if (builder[i] == c)
						{
							builder.Insert(i, '\\');
							length++;
                        }
                    }
				}
				return builder.ToString();
			}
			return text;
        }

		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(User user)
			=> $"<@{user.Id}>";
		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(string userId)
			=> $"<@{userId}>";

		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Channel(Channel channel)
			=> $"<#{channel.Id}>";
		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Channel(string channelId)
			=> $"<#{channelId}>";

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
		public static string Text(string text, bool escape = true, bool bold = false, bool italics = false, bool underline = false, bool strikeout = false)
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

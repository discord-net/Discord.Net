using System.Text;

namespace Discord
{
    public static class MessageHelper
	{
		private static char[] specialChars = new char[] {'_', '*', '\\' }; //Backslash must always be last!

		/// <summary> Removes all special formatting characters from the provided text. </summary>
		public static string Escape(string text)
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
	}
}

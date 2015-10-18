namespace Discord
{
    public static class Mention
	{
		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(User user)
			=> $"<@{user.Id}>";
		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(Member member)
			=> $"<@{member.UserId}>";
		/// <summary> Returns the string used to create a user mention. </summary>
		public static string User(string userId)
			=> $"<@{userId}>";

		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Channel(Channel channel)
			=> $"<#{channel.Id}>";
		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Channel(string channelId)
			=> $"<#{channelId}>";

		/// <summary> Returns the string used to create a channel mention. </summary>
		public static string Everyone()
			=> $"@everyone";
	}
}

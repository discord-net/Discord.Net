namespace Discord
{
    public static class LogExtensions
    {
		public static LogService Log(this DiscordClient client, bool required = true)
			=> client.GetService<LogService>(required);
	}
}

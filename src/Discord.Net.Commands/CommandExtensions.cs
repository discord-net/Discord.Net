namespace Discord.Commands
{
    public static class CommandExtensions
    {
		public static CommandService Commands(this DiscordClient client)
			=> client.GetService<CommandService>();
    }
}

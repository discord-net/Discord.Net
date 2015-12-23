namespace Discord.Commands
{
    public static class CommandExtensions
    {
		public static CommandService Commands(this DiscordClient client, bool required = true)
			=> client.Services.Get<CommandService>(required);
    }
}

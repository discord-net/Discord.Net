using System;

namespace Discord.Commands
{
    public static class CommandExtensions
    {
        public static DiscordClient UsingCommands(this DiscordClient client, CommandServiceConfig config = null)
        {
            client.Services.Add(new CommandService(config));
            return client;
        }
        public static DiscordClient UsingCommands(this DiscordClient client, Action<CommandServiceConfig> configFunc = null)
        {
            var config = new CommandServiceConfig();
            configFunc(config);
            client.Services.Add(new CommandService(config));
            return client;
        }
        public static CommandService Commands(this DiscordClient client, bool required = true)
			=> client.Services.Get<CommandService>(required);
    }
}

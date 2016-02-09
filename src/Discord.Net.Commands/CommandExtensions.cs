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
        public static DiscordClient UsingCommands(this DiscordClient client, Action<CommandServiceConfigBuilder> configFunc = null)
        {
            var builder = new CommandServiceConfigBuilder();
            configFunc(builder);
            client.Services.Add(new CommandService(builder));
            return client;
        }
    }
}

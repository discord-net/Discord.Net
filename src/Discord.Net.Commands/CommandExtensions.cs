using System;

namespace Discord.Commands
{
    public static class CommandExtensions
    {
        public static DiscordClient UsingCommands(this DiscordClient client, CommandServiceConfig config = null)
        {
            client.AddService(new CommandService(config));
            return client;
        }
        public static DiscordClient UsingCommands(this DiscordClient client, Action<CommandServiceConfigBuilder> configFunc = null)
        {
            var builder = new CommandServiceConfigBuilder();
            configFunc(builder);
            client.AddService(new CommandService(builder));
            return client;
        }
    }
}

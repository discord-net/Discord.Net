using Discord.WebSocket;

namespace Discord.Commands
{
    public class ShardedCommandContext : SocketCommandContext, ICommandContext
    {
        public new DiscordShardedClient Client { get; }

        public ShardedCommandContext(DiscordShardedClient client, SocketUserMessage msg)
            : base(client.GetShard(GetShardId(client, (msg.Channel as SocketGuildChannel)?.Guild)), msg)
        {
            Client = client;
        }

        private static int GetShardId(DiscordShardedClient client, IGuild guild)
            => guild == null ? 0 : client.GetShardIdFor(guild);

        //ICommandContext
        IDiscordClient ICommandContext.Client => Client;
    }
}

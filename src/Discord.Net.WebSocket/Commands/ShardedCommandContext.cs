using Discord.WebSocket;

namespace Discord.Commands
{
    /// <summary> The sharded variant of <see cref="ICommandContext"/>, which may contain the client, user, guild, channel, and message. </summary>
    public class ShardedCommandContext : SocketCommandContext, ICommandContext
    {
        #region ShardedCommandContext
        /// <summary> Gets the <see cref="DiscordShardedClient"/> that the command is executed with. </summary>
        public new DiscordShardedClient Client { get; }

        public ShardedCommandContext(DiscordShardedClient client, SocketUserMessage msg)
            : base(client.GetShard(GetShardId(client, (msg.Channel as SocketGuildChannel)?.Guild)), msg)
        {
            Client = client;
        }

        /// <summary> Gets the shard ID of the command context. </summary>
        private static int GetShardId(DiscordShardedClient client, IGuild guild)
            => guild == null ? 0 : client.GetShardIdFor(guild);
        #endregion

        #region ICommandContext
        /// <inheritdoc />
        IDiscordClient ICommandContext.Client => Client;
        #endregion
    }
}

using Discord.WebSocket;

namespace Discord.Interactions
{
    /// <summary>
    ///     The sharded variant of <see cref="SocketInteractionContext{TInteraction}"/>.
    /// </summary>
    public class ShardedInteractionContext<TInteraction> : SocketInteractionContext<TInteraction>, IInteractionContext
        where TInteraction : SocketInteraction
    {
        /// <summary>
        ///     Gets the <see cref="DiscordSocketClient"/> that the command will be executed with.
        /// </summary>
        public new DiscordShardedClient Client { get; }

        /// <summary>
        ///     Initializes a <see cref="SocketInteractionContext{TInteraction}"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        public ShardedInteractionContext (DiscordShardedClient client, TInteraction interaction)
            : base(client.GetShard(GetShardId(client, (interaction.User as SocketGuildUser )?.GuildId)), interaction)
        {
            Client = client;
        }

        private static int GetShardId(DiscordShardedClient client, ulong? guildId)
            => guildId.HasValue ? client.GetShardIdFor(guildId.Value) : 0;
    }

    /// <summary>
    ///     The sharded variant of <see cref="SocketInteractionContext"/>.
    /// </summary>
    public class ShardedInteractionContext : ShardedInteractionContext<SocketInteraction>
    {
        /// <summary>
        ///     Initializes a <see cref="ShardedInteractionContext"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        public ShardedInteractionContext(DiscordShardedClient client, SocketInteraction interaction) : base(client, interaction) { }
    }
}

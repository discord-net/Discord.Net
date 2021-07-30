using Discord.WebSocket;

namespace Discord.Commands
{
    /// <summary>
    ///     Represents a WebSocket-based context of a command. This may include the client, guild, channel, user, and message.
    /// </summary>
    public class SocketCommandContext : ICommandContext
    {
        /// <summary>
        ///     Gets the <see cref="DiscordSocketClient" /> that the command is executed with.
        /// </summary>
        public DiscordSocketClient Client { get; }
        /// <summary>
        ///     Gets the <see cref="SocketGuild" /> that the command is executed in.
        /// </summary>
        public SocketGuild Guild { get; }
        /// <inheritdoc/>
        public ulong? GuildId { get; }
        /// <inheritdoc/>
        public Cacheable<IMessageChannel, ulong> Channel { get; }
        /// <summary>
        ///     Gets the <see cref="SocketUser" /> who executed the command.
        /// </summary>
        public SocketUser User { get; }
        /// <summary>
        ///     Gets the <see cref="SocketUserMessage" /> that the command is interpreted from.
        /// </summary>
        public SocketUserMessage Message { get; }

        /// <summary>
        ///     Indicates whether the channel that the command is executed in is a private channel.
        /// </summary>
        public bool IsPrivate => GuildId == null;

        /// <summary>
        ///     Initializes a new <see cref="SocketCommandContext" /> class with the provided client and message.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="msg">The underlying message.</param>
        public SocketCommandContext(DiscordSocketClient client, SocketUserMessage msg)
        {
            Client = client;
            GuildId = msg.GuildId;
            if (msg.GuildId != null)
                Guild = client.GetGuild(msg.GuildId.Value);
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }

        //ICommandContext
        /// <inheritdoc/>
        IDiscordClient ICommandContext.Client => Client;
        /// <inheritdoc/>
        IGuild ICommandContext.Guild => Guild;
        /// <inheritdoc/>
        Cacheable<IMessageChannel, ulong> ICommandContext.Channel => Channel;
        /// <inheritdoc/>
        IUser ICommandContext.User => User;
        /// <inheritdoc/>
        IUserMessage ICommandContext.Message => Message;
    }
}

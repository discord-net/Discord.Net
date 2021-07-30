namespace Discord.Commands
{
    /// <summary> The context of a command which may contain the client, user, guild, channel, and message. </summary>
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        public IDiscordClient Client { get; }
        /// <inheritdoc/>
        public IGuild Guild { get; }
        /// <inheritdoc/>
        public ulong? GuildId { get; }
        /// <inheritdoc/>
        public Cacheable<IMessageChannel, ulong> Channel { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public IUserMessage Message { get; }

        /// <summary> Indicates whether the channel that the command is executed in is a private channel. </summary>
        public bool IsPrivate => GuildId == null;

        /// <summary>
        ///     Initializes a new <see cref="CommandContext" /> class with the provided client and message.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="msg">The underlying message.</param>
        public CommandContext(IDiscordClient client, IUserMessage msg, IGuild guild)
        {
            Client = client;
            GuildId = msg.GuildId;
            Guild = guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }
    }
}

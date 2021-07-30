namespace Discord.Commands
{
    /// <summary>
    ///     Represents a context of a command. This may include the client, guild, channel, user, and message.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        ///     Gets the <see cref="IDiscordClient" /> that the command is executed with.
        /// </summary>
        IDiscordClient Client { get; }
        /// <summary>
        ///     Gets the <see cref="IGuild" /> that the command is executed in.
        /// </summary>
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the guild id that the command is executed in if it was in one.
        /// </summary>
        ulong? GuildId { get; }
        /// <summary>
        ///     Gets the <see cref="IMessageChannel" /> that the command is executed in if cached;
        ///     otherwise just the channel id.
        /// </summary>
        Cacheable<IMessageChannel, ulong> Channel { get; }
        /// <summary>
        ///     Gets the <see cref="IUser" /> who executed the command.
        /// </summary>
        IUser User { get; }
        /// <summary>
        ///     Gets the <see cref="IUserMessage" /> that the command is interpreted from.
        /// </summary>
        IUserMessage Message { get; }
    }
}

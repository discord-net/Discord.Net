namespace Discord;

/// <summary>
///     Represents a Discord thread user.
/// </summary>
public interface IThreadUser : IMentionable
{
    /// <summary>
    ///     Gets the <see cref="IThreadChannel"/> this user is in.
    /// </summary>
    IEntitySource<IThreadChannel, ulong> Thread { get; }

    /// <summary>
    ///     Gets the timestamp for when this user joined this thread.
    /// </summary>
    DateTimeOffset ThreadJoinedAt { get; }

    /// <summary>
    ///     Gets the guild this thread was created in.
    /// </summary>
    IEntitySource<IGuild, ulong> Guild { get; }

    /// <summary>
    ///     Gets the <see cref="IGuildUser"/> on the server this thread was created by.
    /// </summary>
    IEntitySource<IGuildUser, ulong> GuildUser { get; }
}

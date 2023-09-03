namespace Discord;

public interface IChannel : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the name of this channel.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this channel.
    /// </returns>
    string Name { get; }

    /// <summary>
    ///     Gets a collection of users that are able to view the channel or are currently in this channel.
    /// </summary>
    IEntityEnumerableSource<ulong, IUser> Users { get; }
}

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
}

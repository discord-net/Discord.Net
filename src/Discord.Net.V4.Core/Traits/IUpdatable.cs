namespace Discord;

public interface IUpdatable
{
    /// <summary>
    ///     Updates this entity by requesting the up-to-date information from the Discord API.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    ValueTask UpdateAsync(RequestOptions? options = null);
}

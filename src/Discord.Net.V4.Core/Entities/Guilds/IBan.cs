namespace Discord;

/// <summary>
///     Represents a generic ban object.
/// </summary>
public interface IBan : IGuildBanActor, IEntity<ulong>
{
    /// <summary>
    ///     Gets the reason why the user is banned if specified.
    /// </summary>
    /// <returns>
    ///     A string containing the reason behind the ban; <see langword="null" /> if none is specified.
    /// </returns>
    string? Reason { get; }
}

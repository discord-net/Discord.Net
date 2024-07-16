using Discord.Models;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic ban object.
/// </summary>
[Refreshable(nameof(Routes.GetGuildBan))]
[FetchableOfMany(nameof(Routes.GetGuildBans))]
public partial interface IBan :
    ISnowflakeEntity<IBanModel>,
    IBanActor
{
    /// <summary>
    ///     Gets the reason why the user is banned if specified.
    /// </summary>
    /// <returns>
    ///     A string containing the reason behind the ban; <see langword="null" /> if none is specified.
    /// </returns>
    string? Reason { get; }
}

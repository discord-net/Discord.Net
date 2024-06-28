using Discord.Models;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic ban object.
/// </summary>
public interface IBan :
    IGuildBanActor,
    IRefreshable<IBan, ulong, IBanModel>
{
    static IApiOutRoute<IBanModel> IRefreshable<IBan, ulong, IBanModel>.RefreshRoute(IBan self, ulong id)
        => Routes.GetGuildBan(self.Require<IGuild>(), id);

    /// <summary>
    ///     Gets the reason why the user is banned if specified.
    /// </summary>
    /// <returns>
    ///     A string containing the reason behind the ban; <see langword="null" /> if none is specified.
    /// </returns>
    string? Reason { get; }
}

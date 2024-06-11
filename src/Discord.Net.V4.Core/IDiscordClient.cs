
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic Discord client.
/// </summary>
public interface IDiscordClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ILoadableEntity<ulong, ISelfUser> CurrentUser { get; }

    IRootEntitySource<IGuildEntitySource, ulong, IGuild> Guilds { get; }
    IGuildEntitySource Guild(ulong id) => Guilds[id];

    IRestApiClient RestApiClient { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
}

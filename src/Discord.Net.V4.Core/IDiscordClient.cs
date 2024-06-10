
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

    IRootEntitySource<IGuildsEntitySource<IGuild>, ulong, IGuild> Guilds { get; }
    IGuildsEntitySource<IGuild> Guild(ulong id) => Guilds[id];

    IRestApiClient RestApiClient { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
}

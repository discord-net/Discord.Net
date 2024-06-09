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
    IEntitySource<ulong, ISelfUser> CurrentUser { get; }

    IRestApiClient RestApiClient { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
}

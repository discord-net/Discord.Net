using Discord.Rest;

namespace Discord.Rest;

/// <summary>
///     An interface that represents a client provider for Rest-based clients.
/// </summary>
public interface IRestClientProvider
{
    /// <summary>
    ///     Gets the Rest client of this provider.
    /// </summary>
    DiscordRestClient RestClient { get; }
}

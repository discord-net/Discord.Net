using Discord.Models;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic Discord client.
/// </summary>
public interface IDiscordClient :
    IAsyncDisposable
{
    IRestApiClient RestApiClient { get; }

    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ICurrentUserActor CurrentUser { get; }

    PagedIndexableGuildLink Guilds { get; }

    IndexableChannelLink Channels { get; }

    IndexableUserLink Users { get; }

    IndexableWebhookLink Webhooks { get; }

    EnumerableIndexableStickerPackLink StickerPacks { get; }

    IndexableStickerLink Stickers { get; }
    
    IndexableInviteLink Invites { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }

    [return: TypeHeuristic(nameof(Guilds))]
    IGuildActor Guild(ulong id) => Guilds[id];

    [return: TypeHeuristic(nameof(Channels))]
    IChannelActor Channel(ulong id) => Channels[id];

    [return: TypeHeuristic(nameof(Users))]
    IUserActor User(ulong id) => Users[id];

    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
}

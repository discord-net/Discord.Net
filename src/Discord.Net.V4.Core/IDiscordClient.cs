using Discord.Models;
using Discord.Models.Json;
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

    GuildLink.Paged<IPartialGuild, PageUserGuildsParams>.Indexable Guilds { get; }

    ChannelLink.Indexable Channels { get; }

    UserLink.Indexable Users { get; }

    WebhookLink.Indexable Webhooks { get; }

    StickerPackLink.Enumerable.Indexable StickerPacks { get; }

    StickerLink.Indexable Stickers { get; }
    
    InviteLink.Indexable Invites { get; }

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

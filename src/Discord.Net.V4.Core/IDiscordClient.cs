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

    IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds { get; }

    IIndexableActor<IChannelActor, ulong, IChannel> Channels { get; }

    IIndexableActor<IUserActor, ulong, IUser> Users { get; }

    IIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }

    IEnumerableIndexableActor<IStickerPackActor, ulong, IStickerPack> StickerPacks { get; }

    IIndexableActor<IStickerActor, ulong, ISticker> Stickers { get; }

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

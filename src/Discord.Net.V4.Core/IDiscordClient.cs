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
    
    ISoundboardSoundActor.Enumerable.Indexable Sounds { get; }
    
    IApplicationActor.Indexable.WithCurrent Applications { get; }

    IGuildActor.Paged<IPartialGuild, PageUserGuildsParams>.Indexable.WithTemplates Guilds { get; }

    IChannelActor.Indexable Channels { get; }
    
    IThreadChannelActor.Indexable Threads { get; }

    IUserActor.Indexable.WithCurrent Users { get; }

    IWebhookActor.Indexable Webhooks { get; }

    IStickerPackActor.Enumerable.Indexable StickerPacks { get; }

    IStickerActor.Indexable Stickers { get; }
    
    IInviteActor.Indexable Invites { get; }
    
    IInteractionActor.ClientLink Interactions { get; }

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
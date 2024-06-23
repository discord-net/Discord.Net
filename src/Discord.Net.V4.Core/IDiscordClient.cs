using Discord.Models;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic Discord client.
/// </summary>
public interface IDiscordClient :
    IDisposable,
    IAsyncDisposable,
    IEntityProvider<IPartialGuild, IPartialGuildModel>,
    IEntityProvider<IGuildMember, IMemberModel>,
    IEntityProvider<IDMChannel, IDMChannelModel>,
    IEntityProvider<IStageInstance, IStageInstanceModel>,
    IEntityProvider<IGuildChannel, IGuildChannelModel>,
    IEntityProvider<IInvite, IInviteModel>,
    IEntityProvider<IUser, IUserModel>,
    IEntityProvider<IMessage, IMessageModel>
{
    IRestApiClient RestApiClient { get; }

    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ILoadableSelfUserActor SelfUser { get; }

    IPagedIndexableActor<ILoadableGuildActor, ulong, IGuild, IPartialGuild> Guilds { get; }

    IIndexableActor<ILoadableChannelActor, ulong, IChannel> Channels { get; }

    IIndexableActor<ILoadableUserActor, ulong, IUser> Users { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
    ILoadableGuildActor Guild(ulong id) => Guilds[id];
    ILoadableChannelActor Channel(ulong id) => Channels[id];
    ILoadableUserActor User(ulong id) => Users[id];

    ILoadableWebhookActor Webhook(ulong id, string? token = null);
}

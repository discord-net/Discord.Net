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
    ISelfUserActor SelfUser { get; }

    IPagedIndexableActor<IGuildActor, ulong, IGuild, IPartialGuild, PageUserGuildsParams> Guilds { get; }

    IIndexableActor<IChannelActor, ulong, IChannel> Channels { get; }

    IIndexableActor<IUserActor, ulong, IUser> Users { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
    IGuildActor Guild(ulong id) => Guilds[id];
    IChannelActor Channel(ulong id) => Channels[id];
    IUserActor User(ulong id) => Users[id];

    IWebhookActor Webhook(ulong id, string? token = null);
}

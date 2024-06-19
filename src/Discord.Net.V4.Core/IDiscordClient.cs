
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
    IEntityProvider<IUser, IUserModel>
{
    IRestApiClient RestApiClient { get; }

    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ILoadableSelfUserActor SelfUser { get; }

    IPagedLoadableRootActor<ILoadableGuildActor, ulong, IGuild, IPartialGuild> Guilds { get; }
    ILoadableGuildActor Guild(ulong id) => Guilds[id];

    IRootActor<ILoadableChannelActor, ulong, IChannel> Channels { get; }
    ILoadableChannelActor Channel(ulong id) => Channels[id];

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
}


using Discord.Extensions;
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
    IEntityProvider<IStageInstance, IStageInstanceModel>
{
    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ILoadableSelfUserActor<ISelfUser> SelfUser { get; }

    IPagedLoadableRootActor<ILoadableGuildActor<IGuild>, ulong, IGuild, IPartialGuild> Guilds { get; }
    ILoadableGuildActor<IGuild> Guild(ulong id) => Guilds[id];

    IRootActor<ILoadableChannelActor<IChannel>, ulong, IChannel> Channels { get; }
    ILoadableChannelActor<IChannel> Channel(ulong id) => Channels[id];

    IRestApiClient RestApiClient { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }

    async Task Test()
    {

    }
}


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
    IEntityProvider<IGuildUser, IMemberModel>,
    IEntityProvider<IDMChannel, IDMChannelModel>
{
    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ILoadableCurrentUserEntitySource<ISelfUser> CurrentUser { get; }

    IRootEntitySource<ILoadableGuildEntitySource<IGuild>, ulong, IGuild> Guilds { get; }
    ILoadableGuildEntitySource<IGuild> Guild(ulong id) => Guilds[id];

    IRootEntitySource<ILoadableChannelEntitySource<IChannel>, ulong, IChannel> Channels { get; }
    ILoadableChannelEntitySource<IChannel> Channel(ulong id) => Channels[id];

    IRestApiClient RestApiClient { get; }

    internal DiscordConfig Config { get; }
    internal RequestOptions DefaultRequestOptions { get; }
}

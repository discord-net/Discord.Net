using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IChannel :
    ISnowflakeEntity,
    IChannelActor,
    IRefreshable<IChannel, ulong, IChannelModel>
{
    ChannelType Type { get; }

    static IApiOutRoute<IChannelModel> IRefreshable<IChannel, ulong, IChannelModel>.RefreshRoute(
        IChannel self,
        ulong id
    ) => Routes.GetChannel(id);
}

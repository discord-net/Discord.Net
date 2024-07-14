using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetChannel))]
public partial interface IChannel :
    ISnowflakeEntity,
    IChannelActor,
    IEntityOf<IChannelModel>
{
    ChannelType Type { get; }
}

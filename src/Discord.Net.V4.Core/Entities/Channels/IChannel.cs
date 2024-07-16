using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetChannel))]
public partial interface IChannel :
    ISnowflakeEntity<IChannelModel>,
    IChannelActor
{
    ChannelType Type { get; }
}

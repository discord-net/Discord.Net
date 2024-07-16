using Discord.Models;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetChannel))]
public partial interface IChannelActor :
    IActor<ulong, IChannel>;

using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel)),
    LinkHierarchicalRoot(Types = [typeof(IDMChannelActor), typeof(IGroupChannelActor)])
]
public partial interface IChannelActor :
    IActor<ulong, IChannel>;

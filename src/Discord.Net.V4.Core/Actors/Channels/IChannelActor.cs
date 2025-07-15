using Discord.Models;
using Discord.Rest;

namespace Discord;

[
    Loadable<Routes.GetChannel>,
    Refreshable,
    RelationshipName("Channel"), 
    LinkHierarchicalRoot(Types = [typeof(IDMChannelActor), typeof(IGroupChannelActor)]),
    PathIdentity(PathParameterType.ChannelId),
]
public partial interface IChannelActor :
    IActor<ulong, IChannel>;

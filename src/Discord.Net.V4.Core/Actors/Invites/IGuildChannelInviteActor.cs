using Discord.Rest;

namespace Discord;

[Creatable<CreateChannelInviteProperties>(nameof(Routes.CreateChannelInvite))]
public partial interface IGuildChannelInviteActor :
    IGuildInviteActor,
    IChannelInviteActor,
    IChannelRelationship<IGuildChannelActor, IGuildChannel>,
    IActor<string, IGuildChannelInvite>
{
    [SourceOfTruth]
    new IGuildChannelActor Channel { get; }
}

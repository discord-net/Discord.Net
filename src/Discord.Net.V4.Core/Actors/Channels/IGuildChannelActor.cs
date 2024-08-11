using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildChannelBase))]
[Modifiable<ModifyGuildChannelProperties>(nameof(Routes.ModifyChannel))]
[Deletable(nameof(Routes.DeleteChannel))]
[Invitable<CreateChannelInviteProperties, IGuildChannelInvite>(nameof(Routes.CreateChannelInvite))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGuildChannelActor :
    IGuildRelationship,
    IInvitableChannelTrait,
    IActor<ulong, IGuildChannel>
{
    [return: TypeHeuristic(nameof(Invites))]
    IGuildChannelInviteActor Invite(string code) => Invites[code];
    IEnumerableIndexableActor<IGuildChannelInviteActor, string, IGuildChannelInvite> Invites { get; }
}

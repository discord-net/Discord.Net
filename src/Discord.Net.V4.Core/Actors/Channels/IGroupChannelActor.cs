using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GroupDMChannel))]
[Modifiable<ModifyGroupDMProperties>(nameof(Routes.ModifyChannel))]
[Invitable<CreateChannelInviteProperties>(nameof(Routes.CreateChannelInvite))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>
{
    // TODO: invites
    // [return: TypeHeuristic(nameof(Invites))]
    // IGuildChannelInviteActor Invite(string code) => Invites[code];
    // IEnumerableIndexableActor<IGuildChannelInviteActor, string, IGuildChannelInvite> Invites { get; }
}

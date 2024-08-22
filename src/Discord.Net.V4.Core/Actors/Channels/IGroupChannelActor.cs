using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GroupDMChannel)),
    Modifiable<ModifyGroupDMProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateGuildChannelProperties>(nameof(Routes.CreateGuildChannel), typeof(IGuildActor)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGroupChannelActor :
    IMessageChannelTrait,
    IInvitableTrait<IChannelInviteActor, IChannelInvite>,
    IActor<ulong, IGroupChannel>;
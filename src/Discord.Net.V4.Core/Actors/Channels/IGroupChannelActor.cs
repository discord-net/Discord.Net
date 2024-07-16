using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GroupDMChannelModel))]
[Modifiable<ModifyGroupDMProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>;

using Discord.Invites;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildChannelModelBase))]
[Modifiable<ModifyGuildChannelProperties>(nameof(Routes.ModifyChannel))]
[Deletable(nameof(Routes.DeleteChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGuildChannelActor :
    IGuildRelationship,
    IInvitableChannelActor,
    IActor<ulong, IGuildChannel>;

using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetChannelMessage))]
[Deletable(nameof(Routes.DeleteMessage))]
[Modifiable<ModifyMessageProperties>(nameof(Routes.ModifyMessage))]
public partial interface IMessageActor :
    IChannelRelationship<IMessageChannelTrait, IMessageChannel>,
    IActor<ulong, IMessage>;

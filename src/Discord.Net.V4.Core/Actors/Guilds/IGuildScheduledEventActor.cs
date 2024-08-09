using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using RSVPType = IPagedActor<ulong, IGuildScheduledEventUser, PageGuildScheduledEventUsersParams>;

[Loadable(nameof(Routes.GetGuildScheduledEvent))]
[Deletable(nameof(Routes.DeleteGuildScheduledEvent))]
[Modifiable<ModifyGuildScheduledEventProperties>(nameof(Routes.ModifyGuildScheduledEvent))]
public partial interface IGuildScheduledEventActor :
    IGuildRelationship,
    IActor<ulong, IGuildScheduledEvent>
{
    RSVPType RSVPs { get; }
}

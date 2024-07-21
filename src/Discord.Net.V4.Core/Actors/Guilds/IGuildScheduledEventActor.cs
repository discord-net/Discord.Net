using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using RSVPType = IEnumerableIndexableActor<IGuildScheduledEventUserActor, ulong, IGuildScheduledEventUser>;

[Loadable(nameof(Routes.GetGuildScheduledEvent))]
[Deletable(nameof(Routes.DeleteGuildScheduledEvent))]
[Modifiable<ModifyGuildScheduledEventProperties>(nameof(Routes.ModifyGuildScheduledEvent))]
public partial interface IGuildScheduledEventActor :
    IGuildRelationship,
    IActor<ulong, IGuildScheduledEvent>
{
    // TODO: make this paged
    RSVPType RSVPs { get; }
}

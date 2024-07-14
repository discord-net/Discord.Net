using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using RSVPType = IEnumerableIndexableActor<ILoadableGuildScheduledEventUserActor<IGuildScheduledEventUser>, ulong,
    IGuildScheduledEventUser>;

public interface ILoadableGuildScheduledEventActor :
    IGuildScheduledEventActor,
    ILoadableEntity<ulong, IGuildScheduledEvent>;

[Deletable(nameof(Routes.DeleteGuildScheduledEvent))]
[Modifiable<ModifyGuildScheduledEventProperties>(nameof(Routes.ModifyGuildScheduledEvent))]
public partial interface IGuildScheduledEventActor :
    IGuildRelationship,
    IActor<ulong, IGuildScheduledEvent>
{
    RSVPType RSVPs { get; }
}

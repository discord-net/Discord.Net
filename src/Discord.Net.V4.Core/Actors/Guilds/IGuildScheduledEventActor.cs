using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildScheduledEvent)),
    Deletable(nameof(Routes.DeleteGuildScheduledEvent)),
    Creatable<CreateGuildScheduledEventProperties>(
        nameof(Routes.CreateGuildScheduledEvent),
        nameof(IGuildActor.ScheduledEvents)
    ),
    Modifiable<ModifyGuildScheduledEventProperties>(nameof(Routes.ModifyGuildScheduledEvent))
]
public partial interface IGuildScheduledEventActor :
    IGuildActor.CanonicalRelationship,
    IActor<ulong, IGuildScheduledEvent>
{
    IGuildScheduledEventUserActor.Paged<PageGuildScheduledEventUsersParams> RSVPs { get; }
}
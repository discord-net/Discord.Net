using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetGuildScheduledEvent))]
[Deletable(nameof(Routes.DeleteGuildScheduledEvent))]
[Creatable<CreateGuildScheduledEventProperties>(nameof(Routes.CreateGuildScheduledEvent))]
[Modifiable<ModifyGuildScheduledEventProperties>(nameof(Routes.ModifyGuildScheduledEvent))]
public partial interface IGuildScheduledEventActor :
    IGuildRelationship,
    IActor<ulong, IGuildScheduledEvent>
{
    PagedGuildScheduledEventUserLink RSVPs { get; }
}

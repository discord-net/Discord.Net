using Discord.Models;
using Discord.Rest;

namespace Discord;

[PagedFetchableOfMany<Routes.ListGuildScheduledEventUsers, PageGuildScheduledEventUsersParams>]
public partial interface IGuildScheduledEventUserActor :
    IGuildScheduledEventActor.CanonicalRelationship,
    IMemberActor.Relationship,
    IActor<ulong, IGuildScheduledEventUser>;

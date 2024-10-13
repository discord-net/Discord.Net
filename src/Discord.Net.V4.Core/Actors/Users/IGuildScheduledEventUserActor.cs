using Discord.Models;

namespace Discord;

public partial interface IGuildScheduledEventUserActor :
    IGuildScheduledEventActor.CanonicalRelationship,
    IMemberActor.Relationship,
    IActor<ulong, IGuildScheduledEventUser>;

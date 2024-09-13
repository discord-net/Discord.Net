using Discord.Models;

namespace Discord;

public partial interface IGuildScheduledEventUserActor :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship,
    IGuildRelationship,
    IEntityProvider<IGuildScheduledEventUser, IGuildScheduledEventUserModel>,
    IActor<ulong, IGuildScheduledEventUser>;

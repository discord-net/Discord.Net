using Discord.Models;

namespace Discord;

public interface IGuildScheduledEventUserActor :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship,
    IGuildRelationship,
    IEntityProvider<IGuildScheduledEventUser, IGuildScheduledEventUserModel>,
    IActor<ulong, IGuildScheduledEventUser>;

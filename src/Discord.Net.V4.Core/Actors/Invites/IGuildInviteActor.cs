using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteInvite))]
public partial interface IGuildInviteActor :
    IInviteActor,
    IGuildRelationship;

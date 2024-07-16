using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteGuildIntegration))]
public partial interface IIntegrationActor :
    IGuildRelationship,
    IActor<ulong, IIntegration>;

using Discord.Models;
using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteGuildIntegration))]
public partial interface IIntegrationActor :
    IGuildRelationship,
    IEntityProvider<IIntegration, IIntegrationModel>,
    IActor<ulong, IIntegration>;
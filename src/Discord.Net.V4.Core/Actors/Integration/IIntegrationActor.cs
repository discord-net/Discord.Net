using Discord.Models;
using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteGuildIntegration))]
public partial interface IIntegrationActor :
    IGuildActor.CanonicalRelationship,
    IEntityProvider<IIntegration, IIntegrationModel>,
    IActor<ulong, IIntegration>;
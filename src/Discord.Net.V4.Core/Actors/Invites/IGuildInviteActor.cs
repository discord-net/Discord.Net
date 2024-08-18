using Discord.Models;
using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteInvite))]
public partial interface IGuildInviteActor :
    IInviteActor,
    IActor<string, IGuildInvite>,
    IEntityProvider<IGuildInvite, IInviteModel>,
    IGuildRelationship
{
    [SourceOfTruth]
    internal new IGuildInvite CreateEntity(IInviteModel model);
}

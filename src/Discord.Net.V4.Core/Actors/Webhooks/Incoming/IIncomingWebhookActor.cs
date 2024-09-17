using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IIncomingWebhookActor :
    IGuildChannelWebhookActor,
    IActor<ulong, IIncomingWebhook>,
    IEntityProvider<IWebhookMessage, IMessageModel>
{
    IIncomingWebhookWithTokenActor this[string token] { get; }
}

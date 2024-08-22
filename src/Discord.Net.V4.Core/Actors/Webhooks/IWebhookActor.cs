using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[Deletable(nameof(Routes.DeleteWebhook))]
public partial interface IWebhookActor :
    IActor<ulong, IWebhook>;

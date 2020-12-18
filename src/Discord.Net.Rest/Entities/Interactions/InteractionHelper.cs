using Discord.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    internal static class InteractionHelper
    {
        internal static async Task<RestUserMessage> SendFollowupAsync(BaseDiscordClient client, API.Rest.CreateWebhookMessageParams args,
            string token, IMessageChannel channel, RequestOptions options = null)
        {
            var model = await client.ApiClient.CreateInteractionFollowupMessage(args, token, options);

            var entity = RestUserMessage.Create(client, channel, client.CurrentUser, model);
            return entity;
        }
    }
}

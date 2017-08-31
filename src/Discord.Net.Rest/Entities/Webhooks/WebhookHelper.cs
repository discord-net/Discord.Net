using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using ImageModel = Discord.API.Image;
using Model = Discord.API.Webhook;

namespace Discord.Rest
{
    internal static class WebhookHelper
    {
        public static async Task<Model> ModifyAsync(IWebhook webhook, BaseDiscordClient client,
            Action<WebhookProperties> func, string webhookToken, RequestOptions options)
        {
            var args = new WebhookProperties();
            func(args);
            var apiArgs = new ModifyWebhookParams
            {
                Avatar = args.Image.IsSpecified ? args.Image.Value?.ToModel() : Optional.Create<ImageModel?>(),
                Name = args.Name
            };

            if (!apiArgs.Avatar.IsSpecified && webhook.AvatarId != null)
                apiArgs.Avatar = new ImageModel(webhook.AvatarId);

            return await client.ApiClient.ModifyWebhookAsync(webhook.Id, apiArgs, webhookToken, options).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IWebhook webhook, BaseDiscordClient client, string webhookToken,
            RequestOptions options)
        {
            await client.ApiClient.DeleteWebhookAsync(webhook.Id, webhookToken, options).ConfigureAwait(false);
        }

    }
}

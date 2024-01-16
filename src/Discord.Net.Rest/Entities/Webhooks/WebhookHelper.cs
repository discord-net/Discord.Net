using Discord.API.Rest;
using System;
using System.Threading.Tasks;
using ImageModel = Discord.API.Image;
using Model = Discord.API.Webhook;

namespace Discord.Rest
{
    internal static class WebhookHelper
    {
        public static Task<Model> ModifyAsync(IWebhook webhook, BaseDiscordClient client,
            Action<WebhookProperties> func, RequestOptions options)
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

            if (args.Channel.IsSpecified)
                apiArgs.ChannelId = args.Channel.Value.Id;
            else if (args.ChannelId.IsSpecified)
                apiArgs.ChannelId = args.ChannelId.Value;

            return client.ApiClient.ModifyWebhookAsync(webhook.Id, apiArgs, options);
        }

        public static Task DeleteAsync(IWebhook webhook, BaseDiscordClient client, RequestOptions options)
            => client.ApiClient.DeleteWebhookAsync(webhook.Id, options);
    }
}

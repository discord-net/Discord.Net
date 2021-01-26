using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.API.Rest;
using Discord.Rest;
using ImageModel = Discord.API.Image;
using WebhookModel = Discord.API.Webhook;

namespace Discord.Webhook
{
    internal static class WebhookClientHelper
    {
        /// <exception cref="InvalidOperationException">Could not find a webhook with the supplied credentials.</exception>
        public static async Task<RestInternalWebhook> GetWebhookAsync(DiscordWebhookClient client, ulong webhookId)
        {
            var model = await client.ApiClient.GetWebhookAsync(webhookId).ConfigureAwait(false);
            if (model == null)
                throw new InvalidOperationException("Could not find a webhook with the supplied credentials.");
            return RestInternalWebhook.Create(client, model);
        }
        public static async Task<ulong> SendMessageAsync(DiscordWebhookClient client, 
            string text, bool isTTS, IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options)
        {
            var args = new CreateWebhookMessageParams(text) { IsTTS = isTTS };
            if (embeds != null)
                args.Embeds = embeds.Select(x => x.ToModel()).ToArray();
            if (username != null)
                args.Username = username;
            if (avatarUrl != null)
                args.AvatarUrl = avatarUrl;
            if (allowedMentions != null)
                args.AllowedMentions = allowedMentions.ToModel();

            var model = await client.ApiClient.CreateWebhookMessageAsync(client.Webhook.Id, args, options: options).ConfigureAwait(false);
            return model.Id;
        }
        public static async Task ModifyMessageAsync(DiscordWebhookClient client, ulong messageId,
            Action<WebhookMessageProperties> func, RequestOptions options)
        {
            var args = new WebhookMessageProperties();
            func(args);

            if (args.AllowedMentions.IsSpecified)
            {
                var allowedMentions = args.AllowedMentions.Value;
                Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds),
                    "A max of 100 role Ids are allowed.");
                Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds),
                    "A max of 100 user Ids are allowed.");

                // check that user flag and user Id list are exclusive, same with role flag and role Id list
                if (allowedMentions?.AllowedTypes != null)
                {
                    if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                        allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                    {
                        throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.",
                            nameof(allowedMentions));
                    }

                    if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                        allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                    {
                        throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.",
                            nameof(allowedMentions));
                    }
                }
            }

            var apiArgs = new ModifyWebhookMessageParams
            {
                Content = args.Content.IsSpecified ? args.Content.Value : Optional.Create<string>(),
                Embeds =
                    args.Embeds.IsSpecified
                        ? args.Embeds.Value.Select(embed => embed.ToModel()).ToArray()
                        : Optional.Create<API.Embed[]>(),
                AllowedMentions = args.AllowedMentions.IsSpecified
                    ? args.AllowedMentions.Value.ToModel()
                    : Optional.Create<API.AllowedMentions>()
            };

            await client.ApiClient.ModifyWebhookMessageAsync(client.Webhook.Id, messageId, apiArgs, options)
                .ConfigureAwait(false);
        }
        public static async Task DeleteMessageAsync(DiscordWebhookClient client, ulong messageId, RequestOptions options)
        {
            await client.ApiClient.DeleteWebhookMessageAsync(client.Webhook.Id, messageId, options).ConfigureAwait(false);
        }
        public static async Task<ulong> SendFileAsync(DiscordWebhookClient client, string filePath, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options, bool isSpoiler)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
                return await SendFileAsync(client, file, filename, text, isTTS, embeds, username, avatarUrl, allowedMentions, options, isSpoiler).ConfigureAwait(false);
        }
        public static async Task<ulong> SendFileAsync(DiscordWebhookClient client, Stream stream, string filename, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options, bool isSpoiler)
        {
            var args = new UploadWebhookFileParams(stream) { Filename = filename, Content = text, IsTTS = isTTS, IsSpoiler = isSpoiler };
            if (username != null)
                args.Username = username;
            if (avatarUrl != null)
                args.AvatarUrl = avatarUrl;
            if (embeds != null)
                args.Embeds = embeds.Select(x => x.ToModel()).ToArray();
            if(allowedMentions != null)
                args.AllowedMentions = allowedMentions.ToModel();
            var msg = await client.ApiClient.UploadWebhookFileAsync(client.Webhook.Id, args, options).ConfigureAwait(false);
            return msg.Id;
        }

        public static async Task<WebhookModel> ModifyAsync(DiscordWebhookClient client,
            Action<WebhookProperties> func, RequestOptions options)
        {
            var args = new WebhookProperties();
            func(args);
            var apiArgs = new ModifyWebhookParams
            {
                Avatar = args.Image.IsSpecified ? args.Image.Value?.ToModel() : Optional.Create<ImageModel?>(),
                Name = args.Name
            };

            if (!apiArgs.Avatar.IsSpecified && client.Webhook.AvatarId != null)
                apiArgs.Avatar = new ImageModel(client.Webhook.AvatarId);

            return await client.ApiClient.ModifyWebhookAsync(client.Webhook.Id, apiArgs, options).ConfigureAwait(false);
        }

        public static async Task DeleteAsync(DiscordWebhookClient client, RequestOptions options)
        {
            await client.ApiClient.DeleteWebhookAsync(client.Webhook.Id, options).ConfigureAwait(false);
        }
    }
}

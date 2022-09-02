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
            string text, bool isTTS, IEnumerable<Embed> embeds, string username, string avatarUrl,
            AllowedMentions allowedMentions, RequestOptions options, MessageComponent components, MessageFlags flags, ulong? threadId = null)
        {
            var args = new CreateWebhookMessageParams
            {
                Content = text,
                IsTTS = isTTS,
                Flags = flags
            };

            if (embeds != null)
                args.Embeds = embeds.Select(x => x.ToModel()).ToArray();
            if (username != null)
                args.Username = username;
            if (avatarUrl != null)
                args.AvatarUrl = avatarUrl;
            if (allowedMentions != null)
                args.AllowedMentions = allowedMentions.ToModel();
            if (components != null)
                args.Components = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray();

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds and none.", nameof(flags));

            var model = await client.ApiClient.CreateWebhookMessageAsync(client.Webhook.Id, args, options: options, threadId: threadId).ConfigureAwait(false);
            return model.Id;
        }

        public static async Task ModifyMessageAsync(DiscordWebhookClient client, ulong messageId,
            Action<WebhookMessageProperties> func, RequestOptions options, ulong? threadId)
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
                    : Optional.Create<API.AllowedMentions>(),
                Components = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
            };

            await client.ApiClient.ModifyWebhookMessageAsync(client.Webhook.Id, messageId, apiArgs, options, threadId)
                .ConfigureAwait(false);
        }
        public static async Task DeleteMessageAsync(DiscordWebhookClient client, ulong messageId, RequestOptions options, ulong? threadId)
        {
            await client.ApiClient.DeleteWebhookMessageAsync(client.Webhook.Id, messageId, options, threadId).ConfigureAwait(false);
        }
        public static async Task<ulong> SendFileAsync(DiscordWebhookClient client, string filePath, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options,
            bool isSpoiler, MessageComponent components, MessageFlags flags = MessageFlags.None, ulong? threadId = null)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
                return await SendFileAsync(client, file, filename, text, isTTS, embeds, username, avatarUrl, allowedMentions, options, isSpoiler, components, flags, threadId).ConfigureAwait(false);
        }
        public static Task<ulong> SendFileAsync(DiscordWebhookClient client, Stream stream, string filename, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options, bool isSpoiler,
            MessageComponent components, MessageFlags flags, ulong? threadId)
            => SendFileAsync(client, new FileAttachment(stream, filename, isSpoiler: isSpoiler), text, isTTS, embeds, username, avatarUrl, allowedMentions, components, options, flags, threadId);

        public static Task<ulong> SendFileAsync(DiscordWebhookClient client, FileAttachment attachment, string text, bool isTTS, 
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, 
            MessageComponent components, RequestOptions options, MessageFlags flags, ulong? threadId)
            => SendFilesAsync(client, new FileAttachment[] { attachment }, text, isTTS, embeds, username, avatarUrl, allowedMentions, components, options, flags, threadId);

        public static async Task<ulong> SendFilesAsync(DiscordWebhookClient client,
            IEnumerable<FileAttachment> attachments, string text, bool isTTS, IEnumerable<Embed> embeds, string username, 
            string avatarUrl, AllowedMentions allowedMentions, MessageComponent components, RequestOptions options,
            MessageFlags flags, ulong? threadId)
        {
            embeds ??= Array.Empty<Embed>();
           
            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Count(), 10, nameof(embeds), "A max of 10 embeds are allowed.");

            foreach (var attachment in attachments)
            {
                Preconditions.NotNullOrEmpty(attachment.FileName, nameof(attachment.FileName), "File Name must not be empty or null");
            }

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (allowedMentions != null && allowedMentions.AllowedTypes.HasValue)
            {
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users) &&
                    allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(allowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles) &&
                    allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(allowedMentions));
                }
            }

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds and none.", nameof(flags));

            var args = new UploadWebhookFileParams(attachments.ToArray()) 
            {
                AvatarUrl = avatarUrl, 
                Username = username, Content = text, 
                IsTTS = isTTS, 
                Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified, 
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified, 
                MessageComponents = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                Flags = flags
            };
            var msg = await client.ApiClient.UploadWebhookFileAsync(client.Webhook.Id, args, options, threadId).ConfigureAwait(false);
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

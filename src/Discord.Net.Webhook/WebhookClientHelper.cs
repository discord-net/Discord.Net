using Discord.API.Rest;
using Discord.Rest;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            AllowedMentions allowedMentions, RequestOptions options, MessageComponent components,
            MessageFlags flags, ulong? threadId = null, string threadName = null, ulong[] appliedTags = null,
            PollProperties poll = null)
        {
            var args = new CreateWebhookMessageParams
            {
                Content = text,
                IsTTS = isTTS,
                Flags = flags
            };

            Preconditions.WebhookMessageAtLeastOneOf(text, components, embeds?.ToArray(), poll: poll);
            Preconditions.ValidatePoll(poll);

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
            if (threadName is not null)
                args.ThreadName = threadName;
            if (appliedTags != null)
                args.AppliedTags = appliedTags;
            if (poll != null)
                args.Poll = poll.ToModel();

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds and none.", nameof(flags));

            var model = await client.ApiClient.CreateWebhookMessageAsync(client.Webhook.Id, args, options: options, threadId: threadId).ConfigureAwait(false);
            return model.Id;
        }

        public static Task ModifyMessageAsync(DiscordWebhookClient client, ulong messageId,
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

            if (!args.Attachments.IsSpecified)
            {
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

                return client.ApiClient.ModifyWebhookMessageAsync(client.Webhook.Id, messageId, apiArgs, options, threadId);
            }
            else
            {
                var attachments = args.Attachments.Value?.ToArray() ?? Array.Empty<FileAttachment>();

                var apiArgs = new UploadWebhookFileParams(attachments)
                {
                    Content = args.Content.IsSpecified ? args.Content.Value : Optional.Create<string>(),
                    Embeds =
                        args.Embeds.IsSpecified
                            ? args.Embeds.Value.Select(embed => embed.ToModel()).ToArray()
                            : Optional.Create<API.Embed[]>(),
                    AllowedMentions = args.AllowedMentions.IsSpecified
                        ? args.AllowedMentions.Value.ToModel()
                        : Optional.Create<API.AllowedMentions>(),
                    MessageComponents = args.Components.IsSpecified ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() : Optional<API.ActionRowComponent[]>.Unspecified,
                };

                return client.ApiClient.ModifyWebhookMessageAsync(client.Webhook.Id, messageId, apiArgs, options, threadId);
            }
        }

        public static Task DeleteMessageAsync(DiscordWebhookClient client, ulong messageId, RequestOptions options, ulong? threadId)
            => client.ApiClient.DeleteWebhookMessageAsync(client.Webhook.Id, messageId, options, threadId);

        public static async Task<ulong> SendFileAsync(DiscordWebhookClient client, string filePath, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options,
            bool isSpoiler, MessageComponent components, MessageFlags flags = MessageFlags.None, ulong? threadId = null, string threadName = null,
            ulong[] appliedTags = null, PollProperties poll = null)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
                return await SendFileAsync(client, file, filename, text, isTTS, embeds, username, avatarUrl, allowedMentions, options, isSpoiler, components, flags, threadId, threadName, appliedTags, poll).ConfigureAwait(false);
        }

        public static Task<ulong> SendFileAsync(DiscordWebhookClient client, Stream stream, string filename, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions, RequestOptions options, bool isSpoiler,
            MessageComponent components, MessageFlags flags, ulong? threadId, string threadName = null, ulong[] appliedTags = null, PollProperties poll = null)
            => SendFileAsync(client, new FileAttachment(stream, filename, isSpoiler: isSpoiler), text, isTTS, embeds, username, avatarUrl, allowedMentions, components, options, flags, threadId, threadName, appliedTags, poll);

        public static Task<ulong> SendFileAsync(DiscordWebhookClient client, FileAttachment attachment, string text, bool isTTS,
            IEnumerable<Embed> embeds, string username, string avatarUrl, AllowedMentions allowedMentions,
            MessageComponent components, RequestOptions options, MessageFlags flags, ulong? threadId, string threadName = null,
            ulong[] appliedTags = null, PollProperties poll = null)
            => SendFilesAsync(client, new FileAttachment[] { attachment }, text, isTTS, embeds, username, avatarUrl, allowedMentions, components, options, flags, threadId, threadName, appliedTags, poll);

        public static async Task<ulong> SendFilesAsync(DiscordWebhookClient client,
            IEnumerable<FileAttachment> attachments, string text, bool isTTS, IEnumerable<Embed> embeds, string username,
            string avatarUrl, AllowedMentions allowedMentions, MessageComponent components, RequestOptions options,
            MessageFlags flags, ulong? threadId, string threadName = null, ulong[] appliedTags = null, PollProperties poll = null)
        {
            embeds ??= Array.Empty<Embed>();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Count(), DiscordConfig.MaxEmbedsPerMessage, nameof(embeds), $"A max of {DiscordConfig.MaxEmbedsPerMessage} Embeds are allowed.");
            Preconditions.ValidatePoll(poll);

            Preconditions.WebhookMessageAtLeastOneOf(text, components, embeds.ToArray(), attachments, poll: poll);

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

            if (flags is not MessageFlags.None and not MessageFlags.SuppressEmbeds and not MessageFlags.SuppressNotification)
                throw new ArgumentException("The only valid MessageFlags are SuppressEmbeds, SuppressNotification and none.", nameof(flags));

            var args = new UploadWebhookFileParams(attachments.ToArray())
            {
                AvatarUrl = avatarUrl,
                Username = username,
                Content = text,
                IsTTS = isTTS,
                Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                MessageComponents = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                Flags = flags,
                ThreadName = threadName,
                AppliedTags = appliedTags,
                Poll = poll?.ToModel() ?? Optional<CreatePollParams>.Unspecified
            };
            var msg = await client.ApiClient.UploadWebhookFileAsync(client.Webhook.Id, args, options, threadId).ConfigureAwait(false);
            return msg.Id;
        }

        public static Task<WebhookModel> ModifyAsync(DiscordWebhookClient client, Action<WebhookProperties> func, RequestOptions options)
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

            return client.ApiClient.ModifyWebhookAsync(client.Webhook.Id, apiArgs, options);
        }

        public static Task DeleteAsync(DiscordWebhookClient client, RequestOptions options)
            => client.ApiClient.DeleteWebhookAsync(client.Webhook.Id, options);
    }
}

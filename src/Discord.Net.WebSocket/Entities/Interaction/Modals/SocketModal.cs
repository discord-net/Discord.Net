using Discord.API.Rest;
using Discord.Net.Rest;
using Discord.Rest;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DataModel = Discord.API.ModalInteractionData;
using ModelBase = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a user submitted <see cref="Discord.Modal"/> received via GateWay.
    /// </summary>
    public class SocketModal : SocketInteraction, IDiscordInteraction, IModalInteraction
    {
        /// <summary>
        ///     Gets the data for this <see cref="Modal"/> interaction.
        /// </summary>
        public new SocketModalData Data { get; set; }

        /// <inheritdoc cref="IModalInteraction.Message"/>
        public SocketUserMessage Message { get; private set; }

        IUserMessage IModalInteraction.Message => Message;

        internal SocketModal(DiscordSocketClient client, ModelBase model, ISocketMessageChannel channel, SocketUser user)
             : base(client, model.Id, channel, user)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            if (model.Message.IsSpecified)
            {
                SocketUser author = null;
                if (Channel is SocketGuildChannel ch)
                {
                    if (model.Message.Value.WebhookId.IsSpecified)
                        author = SocketWebhookUser.Create(ch.Guild, Discord.State, model.Message.Value.Author.Value, model.Message.Value.WebhookId.Value);
                    else if (model.Message.Value.Author.IsSpecified)
                        author = ch.Guild.GetUser(model.Message.Value.Author.Value.Id);
                }
                else if (model.Message.Value.Author.IsSpecified)
                    author = (Channel as SocketChannel)?.GetUser(model.Message.Value.Author.Value.Id);

                author ??= Discord.State.GetOrAddUser(model.Message.Value.Author.Value.Id, _ => SocketGlobalUser.Create(Discord, Discord.State, model.Message.Value.Author.Value));

                Message = SocketUserMessage.Create(Discord, Discord.State, author, Channel, model.Message.Value);
            }

            Data = new SocketModalData(dataModel, client, client.State, client.State.GetGuild(model.GuildId.GetValueOrDefault()), model.User.GetValueOrDefault());
        }

        internal new static SocketModal Create(DiscordSocketClient client, ModelBase model, ISocketMessageChannel channel, SocketUser user)
        {
            var entity = new SocketModal(client, model, channel, user);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public override bool HasResponded { get; internal set; }
        private object _lock = new object();

        /// <inheritdoc/>
        public override async Task RespondWithFilesAsync(
            IEnumerable<FileAttachment> attachments,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null,
            PollProperties poll = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.ValidatePoll(poll);

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

            var response = new API.Rest.UploadInteractionFileParams(attachments?.ToArray())
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Content = text ?? Optional<string>.Unspecified,
                AllowedMentions = allowedMentions != null ? allowedMentions?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                IsTTS = isTTS,
                MessageComponents = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<CreatePollParams>.Unspecified
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond, update, or defer the same interaction twice");
                }
            }

            await InteractionHelper.SendInteractionResponseAsync(Discord, response, this, Channel, options).ConfigureAwait(false);
            HasResponded = true;
        }

        /// <inheritdoc/>
        public override async Task RespondAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null,
            PollProperties poll = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.ValidatePoll(poll);

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

            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new API.InteractionCallbackData
                {
                    Content = text ?? Optional<string>.Unspecified,
                    AllowedMentions = allowedMentions?.ToModel(),
                    Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                    TTS = isTTS,
                    Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified,
                    Components = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                    Poll = poll?.ToModel() ?? Optional<CreatePollParams>.Unspecified
                }
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond, update, or defer twice to the same interaction");
                }
            }

            await InteractionHelper.SendInteractionResponseAsync(Discord, response, this, Channel, options).ConfigureAwait(false);
            HasResponded = true;
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            if (args.AllowedMentions.IsSpecified)
            {
                var allowedMentions = args.AllowedMentions.Value;
                Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions), "A max of 100 role Ids are allowed.");
                Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions), "A max of 100 user Ids are allowed.");
            }

            var embed = args.Embed;
            var embeds = args.Embeds;

            var apiEmbeds = embed.IsSpecified || embeds.IsSpecified ? new List<API.Embed>() : null;

            if (embed.IsSpecified && embed.Value != null)
            {
                apiEmbeds.Add(embed.Value.ToModel());
            }

            if (embeds.IsSpecified && embeds.Value != null)
            {
                apiEmbeds.AddRange(embeds.Value.Select(x => x.ToModel()));
            }

            Preconditions.AtMost(apiEmbeds?.Count ?? 0, 10, nameof(args.Embeds), "A max of 10 embeds are allowed.");

            // check that user flag and user Id list are exclusive, same with role flag and role Id list
            if (args.AllowedMentions.IsSpecified && args.AllowedMentions.Value != null && args.AllowedMentions.Value.AllowedTypes.HasValue)
            {
                var allowedMentions = args.AllowedMentions.Value;
                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Users)
                && allowedMentions.UserIds != null && allowedMentions.UserIds.Count > 0)
                {
                    throw new ArgumentException("The Users flag is mutually exclusive with the list of User Ids.", nameof(args.AllowedMentions));
                }

                if (allowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.Roles)
                && allowedMentions.RoleIds != null && allowedMentions.RoleIds.Count > 0)
                {
                    throw new ArgumentException("The Roles flag is mutually exclusive with the list of Role Ids.", nameof(args.AllowedMentions));
                }
            }

            if (!args.Attachments.IsSpecified)
            {
                var response = new API.InteractionResponse
                {
                    Type = InteractionResponseType.UpdateMessage,
                    Data = new API.InteractionCallbackData
                    {
                        Content = args.Content,
                        AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                        Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                        Components = args.Components.IsSpecified
                            ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Array.Empty<API.ActionRowComponent>()
                            : Optional<API.ActionRowComponent[]>.Unspecified,
                        Flags = args.Flags.IsSpecified ? args.Flags.Value ?? Optional<MessageFlags>.Unspecified : Optional<MessageFlags>.Unspecified
                    }
                };

                await InteractionHelper.SendInteractionResponseAsync(Discord, response, this, Channel, options).ConfigureAwait(false);
            }
            else
            {
                var attachments = args.Attachments.Value?.ToArray() ?? Array.Empty<FileAttachment>();

                var response = new API.Rest.UploadInteractionFileParams(attachments)
                {
                    Type = InteractionResponseType.UpdateMessage,
                    Content = args.Content,
                    AllowedMentions = args.AllowedMentions.IsSpecified ? args.AllowedMentions.Value?.ToModel() : Optional<API.AllowedMentions>.Unspecified,
                    Embeds = apiEmbeds?.ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    MessageComponents = args.Components.IsSpecified
                        ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Array.Empty<API.ActionRowComponent>()
                        : Optional<API.ActionRowComponent[]>.Unspecified,
                    Flags = args.Flags.IsSpecified ? args.Flags.Value ?? Optional<MessageFlags>.Unspecified : Optional<MessageFlags>.Unspecified
                };

                await InteractionHelper.SendInteractionResponseAsync(Discord, response, this, Channel, options).ConfigureAwait(false);
            }

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond, update, or defer twice to the same interaction");
                }
            }

            HasResponded = true;
        }

        /// <inheritdoc/>
        public override Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null,
            PollProperties poll = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.ValidatePoll(poll);

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                Components = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<CreatePollParams>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override Task<RestFollowupMessage> FollowupWithFilesAsync(
            IEnumerable<FileAttachment> attachments,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null,
            PollProperties poll = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");
            Preconditions.ValidatePoll(poll);

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

            var flags = MessageFlags.None;

            if (ephemeral)
                flags |= MessageFlags.Ephemeral;

            var args = new API.Rest.UploadWebhookFileParams(attachments.ToArray())
            {
                Flags = flags,
                Content = text,
                IsTTS = isTTS,
                Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                MessageComponents = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<CreatePollParams>.Unspecified
            };
            return InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        /// <remarks>     
        ///     Acknowledges this interaction with the <see cref="InteractionResponseType.DeferredUpdateMessage"/> if the modal was created
        ///     in a response to a message component interaction, <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/> otherwise.
        /// </remarks>
        public override async Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot defer an interaction after {InteractionHelper.ResponseTimeLimit} seconds of no response/acknowledgement");

            var response = new API.InteractionResponse
            {
                Type = Message is not null
                    ? InteractionResponseType.DeferredUpdateMessage
                    : InteractionResponseType.DeferredChannelMessageWithSource,
                Data = ephemeral ? new API.InteractionCallbackData { Flags = MessageFlags.Ephemeral } : Optional<API.InteractionCallbackData>.Unspecified
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond or defer twice to the same interaction");
                }
            }

            await Discord.Rest.ApiClient.CreateInteractionResponseAsync(response, Id, Token, options).ConfigureAwait(false);

            lock (_lock)
            {
                HasResponded = true;
            }
        }

        /// <inheritdoc/>
        public async Task DeferLoadingAsync(bool ephemeral = false, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot defer an interaction after {InteractionHelper.ResponseTimeLimit} seconds of no response/acknowledgement");

            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.DeferredChannelMessageWithSource,
                Data = ephemeral ? new API.InteractionCallbackData { Flags = MessageFlags.Ephemeral } : Optional<API.InteractionCallbackData>.Unspecified
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond or defer twice to the same interaction");
                }
            }

            await Discord.Rest.ApiClient.CreateInteractionResponseAsync(response, Id, Token, options).ConfigureAwait(false);
            HasResponded = true;
        }

        /// <inheritdoc/>
        public override Task RespondWithModalAsync(Modal modal, RequestOptions options = null)
            => throw new NotSupportedException("You cannot respond to a modal with a modal!");

        IModalInteractionData IModalInteraction.Data => Data;
    }
}

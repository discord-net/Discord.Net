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

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a user submitted <see cref="Modal"/>.
    /// </summary>
    public class RestModal : RestInteraction, IDiscordInteraction, IModalInteraction
    {
        internal RestModal(DiscordRestClient client, ModelBase model)
             : base(client, model.Id)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            if (model.Message.IsSpecified && model.ChannelId.IsSpecified)
            {
                Message = RestUserMessage.Create(Discord, Channel, User, model.Message.Value);
            }

            Data = new RestModalData(dataModel, client, Guild);
        }

        internal new static async Task<RestModal> CreateAsync(DiscordRestClient client, ModelBase model, bool doApiCall)
        {
            var entity = new RestModal(client, model);
            await entity.UpdateAsync(client, model, doApiCall);
            return entity;
        }

        private object _lock = new object();

        /// <summary>
        ///     Acknowledges this interaction with the <see cref="InteractionResponseType.DeferredUpdateMessage"/> if the modal was created
        ///     in a response to a message component interaction, <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/> otherwise.
        /// </summary>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public override string Defer(bool ephemeral = false, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot defer an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            var response = new API.InteractionResponse
            {
                Type = Message is not null
                    ? InteractionResponseType.DeferredUpdateMessage
                    : InteractionResponseType.DeferredChannelMessageWithSource,
                Data = new API.InteractionCallbackData
                {
                    Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified
                }
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond or defer twice to the same interaction");
                }
            }

            lock (_lock)
            {
                HasResponded = true;
            }

            return SerializePayload(response);
        }

        /// <summary>
        ///     Defers an interaction and responds with type 5 (<see cref="InteractionResponseType.DeferredChannelMessageWithSource"/>)
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> to send this message ephemerally, otherwise <see langword="false"/>.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public string DeferLoading(bool ephemeral = false, RequestOptions options = null)
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

                HasResponded = true;
            }

            return SerializePayload(response);
        }


        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public override Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent component = null,
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
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ??Optional<API.ActionRowComponent[]>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<API.Rest.CreatePollParams>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="fileStream">The file to upload.</param>
        /// <param name="fileName">The file name of the attachment.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public override Task<RestFollowupMessage> FollowupWithFileAsync(
            Stream fileStream,
            string fileName,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent component = null,
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
            Preconditions.NotNull(fileStream, nameof(fileStream), "File Stream must have data");
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");
            Preconditions.ValidatePoll(poll);

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                File = fileStream is not null ? new MultipartFile(fileStream, fileName) : Optional<MultipartFile>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<API.Rest.CreatePollParams>.Unspecified,
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="filePath">The file to upload.</param>
        /// <param name="fileName">The file name of the attachment.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public override async Task<RestFollowupMessage> FollowupWithFileAsync(
            string filePath,
            string text = null,
            string fileName = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent component = null,
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
            Preconditions.NotNullOrEmpty(filePath, nameof(filePath), "Path must exist");
            Preconditions.ValidatePoll(poll);

            fileName ??= Path.GetFileName(filePath);
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");

            using var fileStream = !string.IsNullOrEmpty(filePath) ? new MemoryStream(File.ReadAllBytes(filePath), false) : null;
            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                File = fileStream != null ? new MultipartFile(fileStream, fileName) : Optional<MultipartFile>.Unspecified,
                Poll = poll?.ToModel() ?? Optional<API.Rest.CreatePollParams>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <summary>
        ///     Responds to an Interaction with type <see cref="InteractionResponseType.ChannelMessageWithSource"/>.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public override string Respond(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent component = null,
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
                    Content = text,
                    AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                    Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                    TTS = isTTS,
                    Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
                    Flags = ephemeral ? MessageFlags.Ephemeral : Optional<MessageFlags>.Unspecified,
                    Poll = poll?.ToModel() ?? Optional<API.Rest.CreatePollParams>.Unspecified
                }
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond twice to the same interaction");
                }
            }

            lock (_lock)
            {
                HasResponded = true;
            }

            return SerializePayload(response);
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
                Poll = poll?.ToModel() ?? Optional<API.Rest.CreatePollParams>.Unspecified
            };
            return InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override Task<RestFollowupMessage> FollowupWithFileAsync(
            FileAttachment attachment,
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
            return FollowupWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll);
        }

        /// <inheritdoc/>
        public override string RespondWithModal(Modal modal, RequestOptions requestOptions = null)
            => throw new NotSupportedException("Modal interactions cannot have modal responces!");

        /// <inheritdoc cref="IModalInteraction.Data"/>
        public new RestModalData Data { get; set; }

        /// <inheritdoc cref="IModalInteraction.Message"/>
        public RestUserMessage Message { get; private set; }

        IUserMessage IModalInteraction.Message => Message;

        IModalInteractionData IModalInteraction.Data => Data;

        /// <inheritdoc />
        Task IModalInteraction.DeferLoadingAsync(bool ephemeral, RequestOptions options)
            => Task.FromResult(DeferLoading(ephemeral, options));

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
    }
}

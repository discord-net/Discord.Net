using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;
using System.IO;
using Discord.Net.Rest;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based message component.
    /// </summary>
    public class RestMessageComponent : RestInteraction, IComponentInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data received with this interaction, contains the button that was clicked.
        /// </summary>
        public new RestMessageComponentData Data { get; }

        /// <summary>
        ///     Gets the message that contained the trigger for this interaction.
        /// </summary>
        public RestUserMessage Message { get; private set; }

        private object _lock = new object();

        internal RestMessageComponent(BaseDiscordClient client, Model model)
            : base(client, model.Id)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            Data = new RestMessageComponentData(dataModel);
        }

        internal new static async Task<RestMessageComponent> CreateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            var entity = new RestMessageComponent(client, model);
            await entity.UpdateAsync(client, model, doApiCall).ConfigureAwait(false);
            return entity;
        }
        internal override async Task UpdateAsync(DiscordRestClient discord, Model model, bool doApiCall)
        {
            await base.UpdateAsync(discord, model, doApiCall).ConfigureAwait(false);

            if (model.Message.IsSpecified && model.ChannelId.IsSpecified)
            {
                if (Message == null)
                {
                    Message = RestUserMessage.Create(Discord, Channel, User, model.Message.Value);
                }
            }
        }
        /// <summary>
        ///     Responds to an Interaction with type <see cref="InteractionResponseType.ChannelMessageWithSource"/>.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public override string Respond(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null)
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
                    Components = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
                }
            };

            if (ephemeral)
                response.Data.Value.Flags = MessageFlags.Ephemeral;

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond, update, or defer twice to the same interaction");
                }

                HasResponded = true;
            }

            return SerializePayload(response);
        }

        /// <summary>
        ///     Updates the message which this component resides in with the type <see cref="InteractionResponseType.UpdateMessage"/>
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A string that contains json to write back to the incoming http request.</returns>
        public string Update(Action<MessageProperties> func, RequestOptions options = null)
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

            bool hasText = args.Content.IsSpecified ? !string.IsNullOrEmpty(args.Content.Value) : !string.IsNullOrEmpty(Message.Content);
            bool hasEmbeds = embed.IsSpecified && embed.Value != null || embeds.IsSpecified && embeds.Value?.Length > 0 || Message.Embeds.Any();

            if (!hasText && !hasEmbeds)
                Preconditions.NotNullOrEmpty(args.Content.IsSpecified ? args.Content.Value : string.Empty, nameof(args.Content));

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

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond, update, or defer twice to the same interaction");
                }

                HasResponded = true;
            }

            return SerializePayload(response);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds.Select(x => x.ToModel()).ToArray(),
                Components = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupWithFileAsync(
            Stream fileStream,
            string fileName,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            Preconditions.NotNull(fileStream, nameof(fileStream), "File Stream must have data");
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");

            using(var file = new FileAttachment(fileStream, fileName))
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupWithFileAsync(
            string filePath,
            string fileName = null,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            Preconditions.NotNullOrEmpty(filePath, nameof(filePath), "Path must exist");

            fileName ??= Path.GetFileName(filePath);
            Preconditions.NotNullOrEmpty(fileName, nameof(fileName), "File Name must not be empty or null");

            using(var file = new FileAttachment(File.OpenRead(filePath), fileName))
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
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
            RequestOptions options = null)
        {
            return FollowupWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupWithFilesAsync(
            IEnumerable<FileAttachment> attachments,
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            MessageComponent components = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            embeds ??= Array.Empty<Embed>();
            if (embed != null)
                embeds = new[] { embed }.Concat(embeds).ToArray();

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds.Length, 10, nameof(embeds), "A max of 10 embeds are allowed.");

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

            var args = new API.Rest.UploadWebhookFileParams(attachments.ToArray()) { Flags = flags, Content = text, IsTTS = isTTS, Embeds = embeds.Any() ? embeds.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified, AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified, MessageComponents = components?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified };
            return await InteractionHelper.SendFollowupAsync(Discord, args, Token, Channel, options).ConfigureAwait(false);
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
        /// 
        /// </summary>
        /// <param name="ephemeral"></param>
        /// <param name="options"></param>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override string Defer(bool ephemeral = false, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot defer an interaction after {InteractionHelper.ResponseTimeLimit} seconds of no response/acknowledgement");

            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.DeferredUpdateMessage,
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
        ///     Responds to the interaction with a modal.
        /// </summary>
        /// <param name="modal">The modal to respond with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A string that contains json to write back to the incoming http request.</returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override string RespondWithModal(Modal modal, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot defer an interaction after {InteractionHelper.ResponseTimeLimit} seconds of no response/acknowledgement");

            var response = new API.InteractionResponse
            {
                Type = InteractionResponseType.Modal,
                Data = new API.InteractionCallbackData
                {
                    CustomId = modal.CustomId,
                    Title = modal.Title,
                    Components = modal.Component.Components.Select(x => new Discord.API.ActionRowComponent(x)).ToArray()
                }
            };

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond or defer twice to the same interaction.");
                }
            }

            lock (_lock)
            {
                HasResponded = true;
            }

            return SerializePayload(response);
        }

        //IComponentInteraction
        /// <inheritdoc/>
        IComponentInteractionData IComponentInteraction.Data => Data;

        /// <inheritdoc/>
        IUserMessage IComponentInteraction.Message => Message;

        /// <inheritdoc />
        Task IComponentInteraction.UpdateAsync(Action<MessageProperties> func, RequestOptions options)
            => Task.FromResult(Update(func, options));

        /// <inheritdoc />
        Task IComponentInteraction.DeferLoadingAsync(bool ephemeral, RequestOptions options)
            => Task.FromResult(DeferLoading(ephemeral, options));
    }
}

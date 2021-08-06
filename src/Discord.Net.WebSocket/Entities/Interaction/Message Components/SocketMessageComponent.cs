using System;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;
using Discord.Rest;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based interaction type for Message Components.
    /// </summary>
    public class SocketMessageComponent : SocketInteraction
    {
        /// <summary>
        ///     The data received with this interaction, contains the button that was clicked.
        /// </summary>
        new public SocketMessageComponentData Data { get; }

        /// <summary>
        ///     The message that contained the trigger for this interaction.
        /// </summary>
        public SocketUserMessage Message { get; private set; }

        internal SocketMessageComponent(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
            : base(client, model.Id, channel)
        {
            var dataModel = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            this.Data = new SocketMessageComponentData(dataModel);
        }

        new internal static SocketMessageComponent Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            var entity = new SocketMessageComponent(client, model, channel);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.Message.IsSpecified)
            {
                if (this.Message == null)
                {
                    SocketUser author = null;
                    if (this.Channel is SocketGuildChannel channel)
                    {
                        if (model.Message.Value.WebhookId.IsSpecified)
                            author = SocketWebhookUser.Create(channel.Guild, Discord.State, model.Message.Value.Author.Value, model.Message.Value.WebhookId.Value);
                        else if (model.Message.Value.Author.IsSpecified)
                            author = channel.Guild.GetUser(model.Message.Value.Author.Value.Id);
                    }
                    else if (model.Message.Value.Author.IsSpecified)
                        author = (this.Channel as SocketChannel).GetUser(model.Message.Value.Author.Value.Id);

                    this.Message = SocketUserMessage.Create(this.Discord, this.Discord.State, author, this.Channel, model.Message.Value);
                }
                else
                {
                    this.Message.Update(this.Discord.State, model.Message.Value);
                }
            }
        }

        /// <inheritdoc/>
        public override async Task RespondAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");
            if (embeds == null && embed != null)
                embeds = new[] { embed };

            if (Discord.AlwaysAcknowledgeInteractions)
            {
                await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options);
                return;
            }

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");

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
                Type =  InteractionResponseType.ChannelMessageWithSource,
                Data = new API.InteractionCallbackData
                {
                    Content = text ?? Optional<string>.Unspecified,
                    AllowedMentions = allowedMentions?.ToModel(),
                    Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                    TTS = isTTS,
                    Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
                }
            };

            if (ephemeral)
                response.Data.Value.Flags = MessageFlags.Ephemeral;

            await InteractionHelper.SendInteractionResponse(this.Discord, response, this.Id, Token, options);
        }

        /// <summary>
        ///     Updates the message which this component resides in with the type <see cref="InteractionResponseType.UpdateMessage"/>
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A task that represents the asynchronous operation of updating the message.</returns>
        public async Task UpdateAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var args = new MessageProperties();
            func(args);

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (args.AllowedMentions.IsSpecified)
            {
               var allowedMentions = args.AllowedMentions.Value;
               Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions), "A max of 100 role Ids are allowed.");
               Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions), "A max of 100 user Ids are allowed.");
            }

            if (args.Embeds.IsSpecified)
                Preconditions.AtMost(args.Embeds.Value?.Length ?? 0, 10, nameof(args.Embeds), "A max of 10 embeds are allowed.");

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
                    Embeds = args.Embeds.IsSpecified ? args.Embeds.Value?.Select(x => x.ToModel()).ToArray() : Optional<API.Embed[]>.Unspecified,
                    Components = args.Components.IsSpecified
                        ? args.Components.Value?.Components.Select(x => new API.ActionRowComponent(x)).ToArray()
                        : Optional<API.ActionRowComponent[]>.Unspecified,
                    Flags = args.Flags.IsSpecified ? args.Flags.Value ?? Optional<MessageFlags>.Unspecified : Optional<MessageFlags>.Unspecified
                }
            };

            await InteractionHelper.SendInteractionResponse(this.Discord, response, this.Id, this.Token, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupAsync(
            string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowedMentions = null,
            RequestOptions options = null,
            MessageComponent component = null,
            Embed embed = null)
        {
            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (embeds == null && embed != null)
                embeds = new[] { embed };
            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");

            var args = new API.Rest.CreateWebhookMessageParams
            {
                Content = text,
                AllowedMentions = allowedMentions?.ToModel() ?? Optional<API.AllowedMentions>.Unspecified,
                IsTTS = isTTS,
                Embeds = embeds?.Select(x => x.ToModel()).ToArray() ?? Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified,
            };

            if (ephemeral)
                args.Flags = MessageFlags.Ephemeral;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <summary>
        ///     Defers an interaction and responds with type 5 (<see cref="InteractionResponseType.DeferredChannelMessageWithSource"/>)
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> to send this message ephemerally, otherwise <see langword="false"/>.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        public Task DeferLoadingAsync(bool ephemeral = false, RequestOptions options = null)
        {
            var response = new API.InteractionResponse()
            {
                Type = InteractionResponseType.DeferredChannelMessageWithSource,
                Data = ephemeral ? new API.InteractionCallbackData() { Flags = MessageFlags.Ephemeral } : Optional<API.InteractionCallbackData>.Unspecified

            };

            return Discord.Rest.ApiClient.CreateInteractionResponse(response, this.Id, this.Token, options);
        }

        /// <inheritdoc/>
        public override Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
        {
            var response = new API.InteractionResponse()
            {
                Type = InteractionResponseType.DeferredUpdateMessage,
                Data = ephemeral ? new API.InteractionCallbackData() { Flags = MessageFlags.Ephemeral } : Optional<API.InteractionCallbackData>.Unspecified

            };

            return Discord.Rest.ApiClient.CreateInteractionResponse(response, this.Id, this.Token, options);
        }
    }
}

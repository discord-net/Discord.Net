using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;
using Newtonsoft.Json.Linq;
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
                        else
                            author = channel.Guild.GetUser(model.Message.Value.Author.Value.Id);
                    }
                    else
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
        public override async Task RespondAsync(string text = null, bool isTTS = false, Embed[] embeds = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {Type} on a send message function");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (Discord.AlwaysAcknowledgeInteractions)
            {
                await FollowupAsync(text, isTTS, embeds, ephemeral, type, allowedMentions, options);
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


            var response = new API.InteractionResponse()
            {
                Type = type,
                Data = new API.InteractionApplicationCommandCallbackData(text)
                {
                    AllowedMentions = allowedMentions?.ToModel(),
                    Embeds = embeds != null
                        ? embeds.Select(x => x.ToModel()).ToArray()
                        : Optional<API.Embed[]>.Unspecified,
                    TTS = isTTS,
                    Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
                }
            };

            if (ephemeral)
                response.Data.Value.Flags = 64;

            await InteractionHelper.SendInteractionResponse(this.Discord, this.Channel, response, this.Id, Token, options);
        }

        /// <inheritdoc/>
        public override async Task<RestFollowupMessage> FollowupAsync(string text = null, bool isTTS = false, Embed[] embeds = null, bool ephemeral = false,
            InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {type} on a slash command!");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");
            Preconditions.AtMost(embeds?.Length ?? 0, 10, nameof(embeds), "A max of 10 embeds are allowed.");

            var args = new API.Rest.CreateWebhookMessageParams(text)
            {
                AllowedMentions = allowedMentions?.ToModel(),
                IsTTS = isTTS,
                Embeds = embeds != null
                        ? embeds.Select(x => x.ToModel()).ToArray()
                        : Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
            };

            if (ephemeral)
                args.Flags = 64;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

        /// <inheritdoc/>
        public override Task AcknowledgeAsync(RequestOptions options = null)
        {
            var response = new API.InteractionResponse()
            {
                Type = InteractionResponseType.DeferredUpdateMessage,
            };

            return Discord.Rest.ApiClient.CreateInteractionResponse(response, this.Id, this.Token, options);
        }
    }
}

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

        /// <summary>
        ///     Responds to an Interaction.
        /// <para>
        ///     If you have <see cref="DiscordSocketConfig.AlwaysAcknowledgeInteractions"/> set to <see langword="true"/>, You should use
        ///     <see cref="FollowupAsync(string, bool, Embed, InteractionResponseType, AllowedMentions, RequestOptions)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="embed">A <see cref="Embed"/> to send with this response.</param>
        /// <param name="type">The type of response to this Interaction.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <returns>
        ///     The <see cref="IMessage"/> sent as the response. If this is the first acknowledgement, it will return null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public override async Task RespondAsync(string text = null, bool isTTS = false, Embed embed = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {Type} on a send message function");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (Discord.AlwaysAcknowledgeInteractions)
            {
                await FollowupAsync(text, isTTS, embed, ephemeral, type, allowedMentions, options);
                return;
            }

            Preconditions.AtMost(allowedMentions?.RoleIds?.Count ?? 0, 100, nameof(allowedMentions.RoleIds), "A max of 100 role Ids are allowed.");
            Preconditions.AtMost(allowedMentions?.UserIds?.Count ?? 0, 100, nameof(allowedMentions.UserIds), "A max of 100 user Ids are allowed.");

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
                    Embeds = embed != null
                        ? new API.Embed[] { embed.ToModel() }
                        : Optional<API.Embed[]>.Unspecified,
                    TTS = isTTS,
                    Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
                }
            };

            if (ephemeral)
                response.Data.Value.Flags = 64;

            await InteractionHelper.SendInteractionResponse(this.Discord, this.Channel, response, this.Id, Token, options);
        }

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="embed">A <see cref="Embed"/> to send with this response.</param>
        /// <param name="type">The type of response to this Interaction.</param>
        /// /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public override async Task<RestFollowupMessage> FollowupAsync(string text = null, bool isTTS = false, Embed embed = null, bool ephemeral = false,
            InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {type} on a slash command!");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            var args = new API.Rest.CreateWebhookMessageParams(text)
            {
                IsTTS = isTTS,
                Embeds = embed != null
                        ? new API.Embed[] { embed.ToModel() }
                        : Optional<API.Embed[]>.Unspecified,
                Components = component?.Components.Select(x => new API.ActionRowComponent(x)).ToArray() ?? Optional<API.ActionRowComponent[]>.Unspecified
            };

            if (ephemeral)
                args.Flags = 64;

            return await InteractionHelper.SendFollowupAsync(Discord.Rest, args, Token, Channel, options);
        }

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

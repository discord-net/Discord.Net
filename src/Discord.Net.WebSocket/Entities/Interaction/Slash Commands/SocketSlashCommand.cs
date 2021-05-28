using Discord.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.InteractionCreated;
using DataModel = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    public class SocketSlashCommand : SocketInteraction
    {
        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        new public SocketSlashCommandData Data { get; private set; }

        internal SocketSlashCommand(DiscordSocketClient client, Model model)
            : base(client, model.Id)
        {
            var dataModel = model.Data.IsSpecified ?
                (model.Data.Value as JToken).ToObject<DataModel>()
                : null;

            Data = SocketSlashCommandData.Create(client, dataModel, model.Id);
        }

        new internal static SocketInteraction Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketSlashCommand(client, model);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            var data = model.Data.IsSpecified ?
                (model.Data.Value as JToken).ToObject<DataModel>()
                : null;

            this.Data.Update(data);

            base.Update(model);
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
        /// <returns>
        ///     The <see cref="IMessage"/> sent as the response. If this is the first acknowledgement, it will return null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>

        public override async Task<RestUserMessage> RespondAsync(string text = null, bool isTTS = false, Embed embed = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {Type} on a send message function");

            if(type == InteractionResponseType.DeferredUpdateMessage || type == InteractionResponseType.UpdateMessage)
                throw new InvalidOperationException($"Cannot use {Type} on a slash command!");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (Discord.AlwaysAcknowledgeInteractions)
                return await FollowupAsync(text, isTTS, embed, ephemeral, type, allowedMentions, options); // The arguments should be passed? What was i thinking...

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

            return await InteractionHelper.SendInteractionResponse(this.Discord, this.Channel, response, this.Id, Token, options);
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
        /// <returns>
        ///     The sent message.
        /// </returns>
        public override async Task<RestFollowupMessage> FollowupAsync(string text = null, bool isTTS = false, Embed embed = null, bool ephemeral = false,
            InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.Pong || type == InteractionResponseType.DeferredUpdateMessage || type == InteractionResponseType.UpdateMessage)
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
    }
}

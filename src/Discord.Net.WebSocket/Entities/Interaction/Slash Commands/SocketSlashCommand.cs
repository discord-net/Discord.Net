using Discord.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based slash command received over the gateway.
    /// </summary>
    public class SocketSlashCommand : SocketInteraction
    {
        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        new public SocketSlashCommandData Data { get; private set; }

        internal SocketSlashCommand(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
            : base(client, model.Id, channel)
        {
            var dataModel = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            ulong? guildId = null;
            if (this.Channel is SocketGuildChannel guildChannel)
                guildId = guildChannel.Guild.Id;

            Data = SocketSlashCommandData.Create(client, dataModel, model.Id, guildId);
        }

        new internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            var entity = new SocketSlashCommand(client, model, channel);
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            var data = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            this.Data.Update(data);

            base.Update(model);
        }

        /// <inheritdoc/>
        public override async Task RespondAsync(Embed[] embeds = null, string text = null, bool isTTS = false, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.Pong)
                throw new InvalidOperationException($"Cannot use {Type} on a send message function");

            if(type == InteractionResponseType.DeferredUpdateMessage || type == InteractionResponseType.UpdateMessage)
                throw new InvalidOperationException($"Cannot use {Type} on a slash command!");

            if (!IsValidToken)
                throw new InvalidOperationException("Interaction token is no longer valid");

            if (Discord.AlwaysAcknowledgeInteractions)
            {
                await FollowupAsync(embeds, text, isTTS, ephemeral, type, allowedMentions, options, component);
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
        public override async Task<RestFollowupMessage> FollowupAsync(Embed[] embeds = null, string text = null, bool isTTS = false, bool ephemeral = false,
            InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        {
            if (type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.DeferredChannelMessageWithSource || type == InteractionResponseType.Pong || type == InteractionResponseType.DeferredUpdateMessage || type == InteractionResponseType.UpdateMessage)
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
                Type = InteractionResponseType.DeferredChannelMessageWithSource,
            };

            return Discord.Rest.ApiClient.CreateInteractionResponse(response, this.Id, this.Token, options);
        }
    }
}

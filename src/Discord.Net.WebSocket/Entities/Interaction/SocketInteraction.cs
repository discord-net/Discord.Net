using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an Interaction recieved over the gateway.
    /// </summary>
    public abstract class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        /// <summary>
        ///     The <see cref="ISocketMessageChannel"/> this interaction was used in.
        /// </summary>
        public ISocketMessageChannel Channel { get; private set; }

        /// <summary>
        ///     The <see cref="SocketUser"/> who triggered this interaction.
        /// </summary>
        public SocketUser User { get; private set; }

        /// <summary>
        ///     The type of this interaction.
        /// </summary>
        public InteractionType Type { get; private set; }

        /// <summary>
        ///     The token used to respond to this interaction.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        ///     The data sent with this interaction.
        /// </summary>
        public IDiscordInteractionData Data { get; private set; }

        /// <summary>
        ///     The version of this interaction.
        /// </summary>
        public int Version { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        /// <summary>
        ///     <see langword="true"/> if the token is valid for replying to, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValidToken
            => CheckToken();

        private ulong? GuildId { get; set; }

        internal SocketInteraction(DiscordSocketClient client, ulong id, ISocketMessageChannel channel)
            : base(client, id)
        {
            this.Channel = channel;
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            if (model.Type == InteractionType.ApplicationCommand)
                return SocketSlashCommand.Create(client, model, channel);
            if (model.Type == InteractionType.MessageComponent)
                return SocketMessageComponent.Create(client, model, channel);
            else
                return null;
        }

        internal virtual void Update(Model model)
        {
            this.Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;

            this.GuildId = model.GuildId.ToNullable();
            this.Token = model.Token;
            this.Version = model.Version;
            this.Type = model.Type;

            if (this.User == null)
            {
                if (model.Member.IsSpecified && model.GuildId.IsSpecified)
                {
                    this.User = SocketGuildUser.Create(Discord.State.GetGuild(this.GuildId.Value), Discord.State, model.Member.Value);
                }
                else
                {
                    this.User = SocketGlobalUser.Create(this.Discord, this.Discord.State, model.User.Value);
                }
            }
        }

        /// <summary>
        ///     Responds to an Interaction.
        /// <para>
        ///     If you have <see cref="DiscordSocketConfig.AlwaysAcknowledgeInteractions"/> set to <see langword="true"/>, You should use
        ///     <see cref="FollowupAsync(string, bool, Embed[], bool, InteractionResponseType, AllowedMentions, RequestOptions, MessageComponent)"/> instead.
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
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public Task RespondAsync(string text = null, bool isTTS = false, Embed embed = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
            => RespondAsync(text, isTTS, new Embed[] { embed }, type, ephemeral, allowedMentions, options, component);

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="embed">A <see cref="Embed"/> to send with this response</param>
        /// <param name="type">The type of response to this Interaction.</param>
        /// /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public Task FollowupAsync(string text = null, bool isTTS = false, Embed embed = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
            => RespondAsync(text, isTTS, new Embed[] { embed }, type, ephemeral, allowedMentions, options, component);
        /// <summary>
        ///     Responds to an Interaction.
        /// <para>
        ///     If you have <see cref="DiscordSocketConfig.AlwaysAcknowledgeInteractions"/> set to <see langword="true"/>, You should use
        ///     <see cref="FollowupAsync(string, bool, Embed[], bool, InteractionResponseType, AllowedMentions, RequestOptions, MessageComponent)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10</param>
        /// <param name="type">The type of response to this Interaction.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>

        public abstract Task RespondAsync(string text = null, bool isTTS = false, Embed[] embeds = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null);

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10</param>
        /// <param name="type">The type of response to this Interaction.</param>
        /// /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, bool isTTS = false, Embed[] embeds = null, bool ephemeral = false,
             InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
             AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null);

        /// <summary>
        ///     Gets the original response for this interaction.
        /// </summary>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the intitial response, or <see langword="null"/> if there is no response.</returns>
        public Task<RestInteractionMessage> GetOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.GetOriginalResponseAsync(this.Discord, this.Channel, this, options);

        /// <summary>
        ///     Acknowledges this interaction with the <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/>.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        public virtual Task AcknowledgeAsync(RequestOptions options = null)
        {
            var response = new API.InteractionResponse()
            {
                Type = InteractionResponseType.DeferredChannelMessageWithSource,
            };

            return Discord.Rest.ApiClient.CreateInteractionResponse(response, this.Id, this.Token, options);
        }

        private bool CheckToken()
        {
            // Tokens last for 15 minutes according to https://discord.com/developers/docs/interactions/slash-commands#responding-to-an-interaction
            return (DateTime.UtcNow - this.CreatedAt.UtcDateTime).TotalMinutes <= 15d;
        }
    }
}

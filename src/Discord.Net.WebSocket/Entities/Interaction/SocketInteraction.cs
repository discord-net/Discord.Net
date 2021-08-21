using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.ApplicationCommandInteractionData;

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
            {
                if (model.ApplicationId != null)
                {
                    var dataModel = model.Data.IsSpecified ?
                        (DataModel)model.Data.Value
                        : null;
                    if (dataModel != null)
                    {
                        if (dataModel.Type.Equals(ApplicationCommandType.User))
                            return SocketUserCommand.Create(client, model, channel);
                        if (dataModel.Type.Equals(ApplicationCommandType.Message))
                            return SocketMessageCommand.Create(client, model, channel);
                    }
                }
                return SocketSlashCommand.Create(client, model, channel);
            }
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
        ///     Responds to an Interaction with type <see cref="InteractionResponseType.ChannelMessageWithSource"/>.
        /// <para>
        ///     If you have <see cref="DiscordSocketConfig.AlwaysAcknowledgeInteractions"/> set to <see langword="true"/>, You should use
        ///     <see cref="FollowupAsync"/> instead.
        /// </para>
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public abstract Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="text">The text of the message to be sent</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null,  bool isTTS = false, bool ephemeral = false,
             AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

        /// <summary>
        ///     Gets the original response for this interaction.
        /// </summary>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public Task<RestInteractionMessage> GetOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.GetOriginalResponseAsync(this.Discord, this.Channel, this, options);

        /// <summary>
        ///     Edits original response for this interaction.
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public async Task<RestInteractionMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await InteractionHelper.ModifyInteractionResponse(this.Discord, this.Token, func, options);
            return RestInteractionMessage.Create(this.Discord, model, this.Token, this.Channel);
        }

        /// <summary>
        ///     Acknowledges this interaction.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        [Obsolete("This method deprecated, please use DeferAsync instead")]
        public Task AcknowledgeAsync(RequestOptions options = null) => DeferAsync(options: options);

        /// <summary>
        ///     Acknowledges this interaction.
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> to send this message ephemerally, otherwise <see langword="false"/>.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        public abstract Task DeferAsync(bool ephemeral = false, RequestOptions options = null);

        private bool CheckToken()
        {
            // Tokens last for 15 minutes according to https://discord.com/developers/docs/interactions/slash-commands#responding-to-an-interaction
            return (DateTime.UtcNow - this.CreatedAt.UtcDateTime).TotalMinutes <= 15d;
        }

        // IDiscordInteraction

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupAsync (string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions,
            RequestOptions options, MessageComponent component, Embed embed)
            => await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.GetOriginalResponseAsync (RequestOptions options)
            => await GetOriginalResponseAsync(options).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.ModifyOriginalResponseAsync (Action<MessageProperties> func, RequestOptions options)
            => await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
    }
}

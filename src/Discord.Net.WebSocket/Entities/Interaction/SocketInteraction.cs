using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using System.IO;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an Interaction received over the gateway.
    /// </summary>
    public abstract class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        #region SocketInteraction
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
            => SnowflakeUtils.FromSnowflake(Id);

        /// <summary>
        ///     <see langword="true"/> if the token is valid for replying to, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValidToken
            => CheckToken();

        internal SocketInteraction(DiscordSocketClient client, ulong id, ISocketMessageChannel channel)
            : base(client, id)
        {
            Channel = channel;
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            if (model.Type == InteractionType.ApplicationCommand)
            {
                var dataModel = model.Data.IsSpecified ?
                        (DataModel)model.Data.Value
                        : null;

                if (dataModel == null)
                    return null;

                return dataModel.Type switch
                {
                    ApplicationCommandType.Slash => SocketSlashCommand.Create(client, model, channel),
                    ApplicationCommandType.Message => SocketMessageCommand.Create(client, model, channel),
                    ApplicationCommandType.User => SocketUserCommand.Create(client, model, channel),
                    _ => null,
                };
            }
            else if (model.Type == InteractionType.MessageComponent)
                return SocketMessageComponent.Create(client, model, channel);
            else if (model.Type == InteractionType.ApplicationCommandAutocomplete)
                return SocketAutocompleteInteraction.Create(client, model, channel);
            else
                return null;
        }

        internal virtual void Update(Model model)
        {
            Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;
            Token = model.Token;
            Version = model.Version;
            Type = model.Type;

            if (User == null)
            {
                if (model.Member.IsSpecified && model.GuildId.IsSpecified)
                {
                    User = SocketGuildUser.Create(Discord.State.GetGuild(model.GuildId.Value), Discord.State, model.Member.Value);
                }
                else
                {
                    User = SocketGlobalUser.Create(Discord, Discord.State, model.User.Value);
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
        /// <param name="options">The request options for this response.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public abstract Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

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
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null,  bool isTTS = false, bool ephemeral = false,
             AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

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
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(string text = null, Stream fileStream = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

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
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(string text = null, string filePath = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

        /// <summary>
        ///     Gets the original response for this interaction.
        /// </summary>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public Task<RestInteractionMessage> GetOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.GetOriginalResponseAsync(Discord, Channel, this, options);

        /// <summary>
        ///     Edits original response for this interaction.
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public async Task<RestInteractionMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await InteractionHelper.ModifyInteractionResponse(Discord, Token, func, options);
            return RestInteractionMessage.Create(Discord, model, Token, Channel);
        }

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
            return (DateTime.UtcNow - CreatedAt.UtcDateTime).TotalMinutes <= 15d;
        }
#endregion

        #region  IDiscordInteraction
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
        #endregion
    }
}

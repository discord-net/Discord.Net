using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using System.IO;
using System.Collections.Generic;

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

        /// <inheritdoc/>
        public string UserLocale { get; private set; }

        /// <inheritdoc/>
        public string GuildLocale { get; private set; }

        /// <summary>
        ///     The version of this interaction.
        /// </summary>
        public int Version { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        ///     Gets whether or not this interaction has been responded to.
        /// </summary>
        /// <remarks>
        ///     This property is locally set -- if you're running multiple bots
        ///     off the same token then this property won't be in sync with them.
        /// </remarks>
        public abstract bool HasResponded { get; internal set; }

        /// <summary>
        ///     <see langword="true"/> if the token is valid for replying to, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValidToken
            => InteractionHelper.CanRespondOrFollowup(this);

        internal SocketInteraction(DiscordSocketClient client, ulong id, ISocketMessageChannel channel)
            : base(client, id)
        {
            Channel = channel;

            CreatedAt = client.UseInteractionSnowflakeDate
                ? SnowflakeUtils.FromSnowflake(Id)
                : DateTime.UtcNow;
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            if (model.Type == InteractionType.ApplicationCommand)
            {
                var dataModel = model.Data.IsSpecified
                    ? (DataModel)model.Data.Value
                    : null;

                if (dataModel == null)
                    return null;

                return dataModel.Type switch
                {
                    ApplicationCommandType.Slash => SocketSlashCommand.Create(client, model, channel),
                    ApplicationCommandType.Message => SocketMessageCommand.Create(client, model, channel),
                    ApplicationCommandType.User => SocketUserCommand.Create(client, model, channel),
                    _ => null
                };
            }

            if (model.Type == InteractionType.MessageComponent)
                return SocketMessageComponent.Create(client, model, channel);

            if (model.Type == InteractionType.ApplicationCommandAutocomplete)
                return SocketAutocompleteInteraction.Create(client, model, channel);

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

            UserLocale = model.UserLocale.IsSpecified
                ? model.UserLocale.Value
                : null;
            GuildLocale = model.GuildLocale.IsSpecified
                ? model.GuildLocale.Value
                : null;
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
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public abstract Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

        /// <summary>
        ///     Responds to this interaction with a file attachment.
        /// </summary>
        /// <param name="fileStream">The file to upload.</param>
        /// <param name="fileName">The file name of the attachment.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public async Task RespondWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
        {
            using (var file = new FileAttachment(fileStream, fileName))
            {
                await RespondWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Responds to this interaction with a file attachment.
        /// </summary>
        /// <param name="filePath">The file to upload.</param>
        /// <param name="fileName">The file name of the attachment.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public async Task RespondWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
        {
            using (var file = new FileAttachment(filePath, fileName))
            {
                await RespondWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Responds to this interaction with a file attachment.
        /// </summary>
        /// <param name="attachment">The attachment containing the file and description.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public Task RespondWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => RespondWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);

        /// <summary>
        ///     Responds to this interaction with a collection of file attachments.
        /// </summary>
        /// <param name="attachments">A collection of attachments to upload.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task RespondWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

        /// <summary>
        ///     Sends a followup message for this interaction.
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
        ///     The sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
             AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

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
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public async Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
        {
            using (var file = new FileAttachment(fileStream, fileName))
            {
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
            }
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
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public async Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
        {
            using (var file = new FileAttachment(filePath, fileName))
            {
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="attachment">The attachment containing the file and description.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public Task<RestFollowupMessage> FollowupWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => FollowupWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);

        /// <summary>
        ///     Sends a followup message for this interaction.
        /// </summary>
        /// <param name="attachments">A collection of attachments to upload.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embeds">A array of embeds to send with this response. Max 10.</param>
        /// <param name="isTTS"><see langword="true"/> if the message should be read out by a text-to-speech reader, otherwise <see langword="false"/>.</param>
        /// <param name="ephemeral"><see langword="true"/> if the response should be hidden to everyone besides the invoker of the command, otherwise <see langword="false"/>.</param>
        /// <param name="allowedMentions">The allowed mentions for this response.</param>
        /// <param name="options">The request options for this response.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

        /// <summary>
        ///     Gets the original response for this interaction.
        /// </summary>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public Task<RestInteractionMessage> GetOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.GetOriginalResponseAsync(Discord, Channel, this, options);

        /// <summary>
        ///     Edits original response for this interaction.
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A <see cref="RestInteractionMessage"/> that represents the initial response.</returns>
        public async Task<RestInteractionMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await InteractionHelper.ModifyInteractionResponseAsync(Discord, Token, func, options);
            return RestInteractionMessage.Create(Discord, model, Token, Channel);
        }

        /// <inheritdoc/>
        public Task DeleteOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.DeleteInteractionResponseAsync(Discord, this, options);

        /// <summary>
        ///     Acknowledges this interaction.
        /// </summary>
        /// <param name="ephemeral"><see langword="true"/> to send this message ephemerally, otherwise <see langword="false"/>.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        public abstract Task DeferAsync(bool ephemeral = false, RequestOptions options = null);

        #endregion

        #region  IDiscordInteraction
        /// <inheritdoc/>
        IUser IDiscordInteraction.User => User;

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.GetOriginalResponseAsync(RequestOptions options)
            => await GetOriginalResponseAsync(options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options)
            => await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task IDiscordInteraction.RespondAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
#if NETCOREAPP3_0_OR_GREATER != true
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(Stream fileStream, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(string filePath, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(filePath, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(FileAttachment attachment, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
#endif
        #endregion
    }
}

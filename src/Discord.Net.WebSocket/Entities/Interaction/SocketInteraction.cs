using Discord.Net;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an Interaction received over the gateway.
    /// </summary>
    public abstract class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        #region SocketInteraction
        /// <summary>
        ///     Gets the <see cref="ISocketMessageChannel"/> this interaction was used in.
        /// </summary>
        /// <remarks>
        ///     If the channel isn't cached, the bot scope isn't used, or the bot doesn't have access to it then
        ///     this property will be <see langword="null"/>.
        /// </remarks>
        public ISocketMessageChannel Channel { get; private set; }

        /// <summary>
        ///     Gets the channel this interaction was used in.
        /// </summary>
        /// <remarks>
        ///     This property can contain a partial channel object. <see langword="null"/> if no channel was passed with the interaction.
        /// </remarks>
        public IMessageChannel InteractionChannel { get; private set; }

        /// <inheritdoc/>
        public ulong? ChannelId { get; private set; }

        /// <summary>
        ///     Gets the <see cref="SocketUser"/> who triggered this interaction.
        /// </summary>
        public SocketUser User { get; private set; }

        /// <inheritdoc/>
        public InteractionType Type { get; private set; }

        /// <inheritdoc/>
        public string Token { get; private set; }

        /// <inheritdoc/>
        public IDiscordInteractionData Data { get; private set; }

        /// <inheritdoc/>
        public string UserLocale { get; private set; }

        /// <inheritdoc/>
        public string GuildLocale { get; private set; }

        /// <inheritdoc/>
        public int Version { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <inheritdoc/>
        public abstract bool HasResponded { get; internal set; }

        /// <summary>
        ///     Gets whether or not the token used to respond to this interaction is valid.
        /// </summary>
        public bool IsValidToken
            => InteractionHelper.CanRespondOrFollowup(this);

        /// <inheritdoc/>
        public bool IsDMInteraction { get; private set; }

        /// <inheritdoc/>
        public ulong? GuildId { get; private set; }

        /// <inheritdoc/>
        public ulong ApplicationId { get; private set; }

        /// <inheritdoc/>
        public InteractionContextType? ContextType { get; private set; }

        /// <inheritdoc/>
        public GuildPermissions Permissions { get; private set; }

        /// <inheritdoc cref="IDiscordInteraction.Entitlements" />
        public IReadOnlyCollection<RestEntitlement> Entitlements { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; private set; }

        internal SocketInteraction(DiscordSocketClient client, ulong id, ISocketMessageChannel channel, SocketUser user)
            : base(client, id)
        {
            Channel = channel;
            User = user;

            CreatedAt = client.UseInteractionSnowflakeDate
                ? SnowflakeUtils.FromSnowflake(Id)
                : DateTime.UtcNow;
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel, SocketUser user)
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
                    ApplicationCommandType.Slash => SocketSlashCommand.Create(client, model, channel, user),
                    ApplicationCommandType.Message => SocketMessageCommand.Create(client, model, channel, user),
                    ApplicationCommandType.User => SocketUserCommand.Create(client, model, channel, user),
                    _ => null
                };
            }

            if (model.Type == InteractionType.MessageComponent)
                return SocketMessageComponent.Create(client, model, channel, user);

            if (model.Type == InteractionType.ApplicationCommandAutocomplete)
                return SocketAutocompleteInteraction.Create(client, model, channel, user);

            if (model.Type == InteractionType.ModalSubmit)
                return SocketModal.Create(client, model, channel, user);

            return null;
        }

        internal virtual void Update(Model model)
        {
            ChannelId = model.Channel.IsSpecified
                ? model.Channel.Value.Id
                : null;

            GuildId = model.GuildId.IsSpecified
                ? model.GuildId.Value
                : null;

            IsDMInteraction = GuildId is null;
            ApplicationId = model.ApplicationId;

            Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;

            Token = model.Token;
            Version = model.Version;
            Type = model.Type;

            UserLocale = model.UserLocale.IsSpecified
                ? model.UserLocale.Value
                : null;

            GuildLocale = model.GuildLocale.IsSpecified
                ? model.GuildLocale.Value
                : null;

            Entitlements = model.Entitlements.Select(x => RestEntitlement.Create(Discord, x)).ToImmutableArray();

            IntegrationOwners = model.IntegrationOwners;
            ContextType = model.ContextType.IsSpecified
                ? model.ContextType.Value
                : null;

            Permissions = new GuildPermissions((ulong)model.ApplicationPermissions);

            InteractionChannel = Channel;
            if (model.Channel.IsSpecified && InteractionChannel is null)
            {
                InteractionChannel = model.Channel.Value.Type switch
                {
                    ChannelType.News or
                        ChannelType.Text or
                        ChannelType.Voice or
                        ChannelType.Stage or
                        ChannelType.NewsThread or
                        ChannelType.PrivateThread or
                        ChannelType.PublicThread or
                        ChannelType.Media or
                        ChannelType.Forum
                        => RestChannel.Create(Discord, model.Channel.Value) as IMessageChannel,
                    ChannelType.DM => RestDMChannel.Create(Discord, model.Channel.Value),
                    ChannelType.Group => RestGroupChannel.Create(Discord, model.Channel.Value),
                    _ => null
                };
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
        /// <param name="poll">A poll to send with the message.</param>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The parameters provided were invalid or the token was invalid.</exception>
        public abstract Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false,
            bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null);

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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public async Task RespondWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
        {
            using (var file = new FileAttachment(fileStream, fileName))
            {
                await RespondWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
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
        /// <param name="poll">A poll to send with the message.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public async Task RespondWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
        {
            using (var file = new FileAttachment(filePath, fileName))
            {
                await RespondWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
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
        /// <param name="poll">A poll to send with the message.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public Task RespondWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
            => RespondWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll);

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
        /// <param name="poll">A poll to send with the message.</param>
        /// <param name="components">A <see cref="MessageComponent"/> to be sent with this response.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task RespondWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null);

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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
             AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null);

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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public async Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
        {
            using (var file = new FileAttachment(fileStream, fileName))
            {
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     The sent message.
        /// </returns>
        public async Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
        {
            using (var file = new FileAttachment(filePath, fileName))
            {
                return await FollowupWithFileAsync(file, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public Task<RestFollowupMessage> FollowupWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null)
            => FollowupWithFilesAsync(new FileAttachment[] { attachment }, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll);

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
        /// <param name="poll">A poll to send with the message.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null, PollProperties poll = null);

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

        /// <summary>
        ///     Responds to this interaction with a <see cref="Modal"/>.
        /// </summary>
        /// <param name="modal">The <see cref="Modal"/> to respond with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A task that represents the asynchronous operation of responding to the interaction.</returns>
        public abstract Task RespondWithModalAsync(Modal modal, RequestOptions options = null);

        /// <inheritdoc/>
        public Task RespondWithPremiumRequiredAsync(RequestOptions options = null)
            => InteractionHelper.RespondWithPremiumRequiredAsync(Discord, Id, Token, options);

        #endregion

        /// <summary>
        ///     Attempts to get the channel this interaction was executed in.
        /// </summary>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of fetching the channel.
        /// </returns>
        public async ValueTask<IMessageChannel> GetChannelAsync(RequestOptions options = null)
        {
            if (Channel != null)
                return Channel;

            if (!ChannelId.HasValue)
                return null;

            try
            {
                return (IMessageChannel)await Discord.GetChannelAsync(ChannelId.Value, options).ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.DiscordCode == DiscordErrorCode.MissingPermissions) { return null; } // bot can't view that channel, return null instead of throwing.
        }

        #region  IDiscordInteraction
        /// <inheritdoc/>
        IReadOnlyCollection<IEntitlement> IDiscordInteraction.Entitlements => Entitlements;

        /// <inheritdoc/>
        IUser IDiscordInteraction.User => User;

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.GetOriginalResponseAsync(RequestOptions options)
            => await GetOriginalResponseAsync(options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options)
            => await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task IDiscordInteraction.RespondAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components,
            Embed embed, RequestOptions options, PollProperties poll)
            => await RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions,
            MessageComponent components, Embed embed, RequestOptions options, PollProperties poll)
            => await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text, Embed[] embeds, bool isTTS,
            bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options, PollProperties poll)
            => await FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
#if NETCOREAPP3_0_OR_GREATER != true
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(Stream fileStream, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options, PollProperties poll)
            => await FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(string filePath, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options, PollProperties poll)
            => await FollowupWithFileAsync(filePath, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(FileAttachment attachment, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options, PollProperties poll)
            => await FollowupWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options, poll).ConfigureAwait(false);
#endif
        #endregion
    }
}

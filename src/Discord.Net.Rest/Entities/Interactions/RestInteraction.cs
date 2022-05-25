using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Newtonsoft.Json;
using Discord.Net;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based interaction.
    /// </summary>
    public abstract class RestInteraction : RestEntity<ulong>, IDiscordInteraction
    {
        // Added so channel & guild methods don't need a client reference
        private Func<RequestOptions, ulong, Task<IRestMessageChannel>> _getChannel;
        private Func<RequestOptions, ulong, Task<RestGuild>> _getGuild;

        /// <inheritdoc/>
        public InteractionType Type { get; private set; }

        /// <inheritdoc/>
        public IDiscordInteractionData Data { get; private set; }

        /// <inheritdoc/>
        public string Token { get; private set; }

        /// <inheritdoc/>
        public int Version { get; private set; }

        /// <summary>
        ///     Gets the user who invoked the interaction.
        /// </summary>
        /// <remarks>
        ///     If this user is an <see cref="RestGuildUser"/> and <see cref="DiscordRestConfig.APIOnRestInteractionCreation"/> is set to false,
        ///     <see cref="RestGuildUser.Guild"/> will return <see langword="null"/>
        /// </remarks>
        public RestUser User { get; private set; }

        /// <inheritdoc/>
        public string UserLocale { get; private set; }

        /// <inheritdoc/>
        public string GuildLocale { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        ///     Gets whether or not the token used to respond to this interaction is valid.
        /// </summary>
        public bool IsValidToken
            => InteractionHelper.CanRespondOrFollowup(this);

        /// <summary>
        ///     Gets the channel that this interaction was executed in.
        /// </summary>
        /// <remarks>
        ///     This property will be <see langword="null"/> if <see cref="DiscordRestConfig.APIOnRestInteractionCreation"/> is set to false.
        ///     Call <see cref="GetChannelAsync"/> to set this property and get the interaction channel.
        /// </remarks>
        public IRestMessageChannel Channel { get; private set; }

        /// <inheritdoc/>
        public ulong? ChannelId { get; private set; }

        /// <summary>
        ///     Gets the guild this interaction was executed in if applicable.
        /// </summary>
        /// <remarks>
        ///     This property will be <see langword="null"/> if <see cref="DiscordRestConfig.APIOnRestInteractionCreation"/> is set to false
        ///     or if the interaction was not executed in a guild.
        /// </remarks>
        public RestGuild Guild { get; private set; }

        /// <inheritdoc/>
        public ulong? GuildId { get; private set; }

        /// <inheritdoc/>
        public bool HasResponded { get; protected set; }

        /// <inheritdoc/>
        public bool IsDMInteraction { get; private set; }

        /// <inheritdoc/>
        public ulong ApplicationId { get; private set; }

        internal RestInteraction(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
            CreatedAt = discord.UseInteractionSnowflakeDate
                ? SnowflakeUtils.FromSnowflake(Id)
                : DateTime.UtcNow;
        }

        internal static async Task<RestInteraction> CreateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            if(model.Type == InteractionType.Ping)
            {
                return await RestPingInteraction.CreateAsync(client, model, doApiCall);
            }

            if (model.Type == InteractionType.ApplicationCommand)
            {
                var dataModel = model.Data.IsSpecified
                    ? (DataModel)model.Data.Value
                    : null;

                if (dataModel == null)
                    return null;

                return dataModel.Type switch
                {
                    ApplicationCommandType.Slash => await RestSlashCommand.CreateAsync(client, model, doApiCall).ConfigureAwait(false),
                    ApplicationCommandType.Message => await RestMessageCommand.CreateAsync(client, model, doApiCall).ConfigureAwait(false),
                    ApplicationCommandType.User => await RestUserCommand.CreateAsync(client, model, doApiCall).ConfigureAwait(false),
                    _ => null
                };
            }

            if (model.Type == InteractionType.MessageComponent)
                return await RestMessageComponent.CreateAsync(client, model, doApiCall).ConfigureAwait(false);

            if (model.Type == InteractionType.ApplicationCommandAutocomplete)
                return await RestAutocompleteInteraction.CreateAsync(client, model, doApiCall).ConfigureAwait(false);

            if (model.Type == InteractionType.ModalSubmit)
                return await RestModal.CreateAsync(client, model, doApiCall).ConfigureAwait(false);

            return null;
        }

        internal virtual async Task UpdateAsync(DiscordRestClient discord, Model model, bool doApiCall)
        {
            ChannelId = model.ChannelId.IsSpecified
                ? model.ChannelId.Value
                : null;

            GuildId = model.GuildId.IsSpecified
                ? model.GuildId.Value
                : null;
            
            IsDMInteraction = GuildId is null;

            Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;
            
            Token = model.Token;
            Version = model.Version;
            Type = model.Type;
            ApplicationId = model.ApplicationId;

            if (Guild is null && GuildId is not null)
            {
                if (doApiCall)
                    Guild = await discord.GetGuildAsync(GuildId.Value);
                else
                {
                    Guild = null;
                    _getGuild = async (opt, ul) => await discord.GetGuildAsync(ul, opt);
                }
            }

            if (User is null)
            {
                if (model.Member.IsSpecified && GuildId is not null)
                {
                    User = RestGuildUser.Create(Discord, Guild, model.Member.Value, GuildId);
                }
                else
                {
                    User = RestUser.Create(Discord, model.User.Value);
                }
            }
            

            if (Channel is null && ChannelId is not null)
            {
                try
                {
                    if (doApiCall)
                        Channel = (IRestMessageChannel)await discord.GetChannelAsync(ChannelId.Value);
                    else
                    {
                        Channel = null;

                        _getChannel = async (opt, ul) =>
                        {
                            if (Guild is null)
                                return (IRestMessageChannel)await discord.GetChannelAsync(ul, opt);

                            // get a guild channel if the guild is set.
                            return (IRestMessageChannel)await Guild.GetChannelAsync(ul, opt);
                        };
                    }
                }
                catch (HttpException x) when (x.DiscordCode == DiscordErrorCode.MissingPermissions) { } // ignore
            }

            UserLocale = model.UserLocale.IsSpecified
                ? model.UserLocale.Value
                : null;

            GuildLocale = model.GuildLocale.IsSpecified
                ? model.GuildLocale.Value
                : null;
        }

        internal string SerializePayload(object payload)
        {
            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                DiscordRestClient.Serializer.Serialize(writer, payload);

            return json.ToString();
        }

        /// <summary>
        ///     Gets the channel this interaction was executed in. Will be a DM channel if the interaction was executed in DM.
        /// </summary>
        /// <remarks>
        ///     Calling this method successfully will populate the <see cref="Channel"/> property.
        ///     After this, further calls to this method will no longer call the API, and depend on the value set in <see cref="Channel"/>.
        /// </remarks>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A Rest channel to send messages to.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no channel can be received.</exception>
        public async Task<IRestMessageChannel> GetChannelAsync(RequestOptions options = null)
        {
            if (Channel is not null)
                return Channel;

            if (IsDMInteraction)
            {
                Channel = await User.CreateDMChannelAsync(options);
            }
            else if (ChannelId is not null)
            {
                Channel = await _getChannel(options, ChannelId.Value) ?? throw new InvalidOperationException("The interaction channel was not able to be retrieved.");
                _getChannel = null; // get rid of it, we don't need it anymore.
            }

            return Channel;
        }

        /// <summary>
        ///     Gets the guild this interaction was executed in if applicable.
        /// </summary>
        /// <remarks>
        ///     Calling this method successfully will populate the <see cref="Guild"/> property.
        ///     After this, further calls to this method will no longer call the API, and depend on the value set in <see cref="Guild"/>.
        /// </remarks>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>The guild this interaction was executed in. <see langword="null"/> if the interaction was executed inside DM.</returns>
        public async Task<RestGuild> GetGuildAsync(RequestOptions options)
        {
            if (GuildId is null)
                return null;

            Guild ??= await _getGuild(options, GuildId.Value);
            _getGuild = null; // get rid of it, we don't need it anymore.

            return Guild;
        }

        /// <inheritdoc/>
        public abstract string Defer(bool ephemeral = false, RequestOptions options = null);
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
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public async Task<RestInteractionMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await InteractionHelper.ModifyInteractionResponseAsync(Discord, Token, func, options);
            return RestInteractionMessage.Create(Discord, model, Token, Channel);
        }
        /// <inheritdoc/>
        public abstract string RespondWithModal(Modal modal, RequestOptions options = null);
        
        /// <inheritdoc/>
        public abstract string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

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
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
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
        ///      A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

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
        ///      A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

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
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null);

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

        /// <inheritdoc/>
        public Task DeleteOriginalResponseAsync(RequestOptions options = null)
            => InteractionHelper.DeleteInteractionResponseAsync(Discord, this, options);

        #region  IDiscordInteraction
        /// <inheritdoc/>
        IUser IDiscordInteraction.User => User;

        /// <inheritdoc/>
        Task IDiscordInteraction.RespondAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => Task.FromResult(Respond(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options));
        /// <inheritdoc/>
        Task IDiscordInteraction.DeferAsync(bool ephemeral, RequestOptions options)
            => Task.FromResult(Defer(ephemeral, options));
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondWithModalAsync(Modal modal, RequestOptions options)
            => Task.FromResult(RespondWithModal(modal, options));
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions,
            MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.GetOriginalResponseAsync(RequestOptions options)
            => await GetOriginalResponseAsync(options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options)
            => await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(Stream fileStream, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral,
            AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(string filePath, string text, string fileName, Embed[] embeds, bool isTTS, bool ephemeral,
            AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(filePath, text, fileName, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFileAsync(FileAttachment attachment, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options)
            => await FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondWithFilesAsync(IEnumerable<FileAttachment> attachments, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options) => throw new NotSupportedException("REST-Based interactions don't support files.");
#if NETCOREAPP3_0_OR_GREATER != true
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondWithFileAsync(Stream fileStream, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options) => throw new NotSupportedException("REST-Based interactions don't support files.");
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondWithFileAsync(string filePath, string fileName, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options) => throw new NotSupportedException("REST-Based interactions don't support files.");
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondWithFileAsync(FileAttachment attachment, string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions options) => throw new NotSupportedException("REST-Based interactions don't support files.");
#endif
        #endregion
    }
}

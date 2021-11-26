using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Newtonsoft.Json;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based interaction.
    /// </summary>
    public abstract class RestInteraction : RestEntity<ulong>, IDiscordInteraction
    {
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
        public RestUser User { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt { get; private set; }

        internal abstract bool _hasResponded { get; set; }

        /// <summary>
        ///     <see langword="true"/> if the token is valid for replying to, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValidToken
            => InteractionHelper.CanRespondOrFollowup(this);

        /// <summary>
        ///     Gets the channel that this interaction was executed in.
        /// </summary>
        public IRestMessageChannel Channel { get; private set; }

        /// <summary>
        ///     Gets the guild this interaction was executed in.
        /// </summary>
        public RestGuild Guild { get; private set; }

        internal RestInteraction(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
            CreatedAt = discord.UseInteractionSnowflakeDate
                ? SnowflakeUtils.FromSnowflake(Id)
                : DateTime.UtcNow;
        }

        internal static async Task<RestInteraction> CreateAsync(DiscordRestClient client, Model model)
        {
            if(model.Type == InteractionType.Ping)
            {
                return await RestPingInteraction.CreateAsync(client, model);
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
                    ApplicationCommandType.Slash => await RestSlashCommand.CreateAsync(client, model).ConfigureAwait(false),
                    ApplicationCommandType.Message => await RestMessageCommand.CreateAsync(client, model).ConfigureAwait(false),
                    ApplicationCommandType.User => await RestUserCommand.CreateAsync(client, model).ConfigureAwait(false),
                    _ => null
                };
            }

            if (model.Type == InteractionType.MessageComponent)
                return await RestMessageComponent.CreateAsync(client, model).ConfigureAwait(false);

            if (model.Type == InteractionType.ApplicationCommandAutocomplete)
                return await RestAutocompleteInteraction.CreateAsync(client, model).ConfigureAwait(false);

            return null;
        }

        internal virtual async Task UpdateAsync(DiscordRestClient discord, Model model)
        {
            Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;
            Token = model.Token;
            Version = model.Version;
            Type = model.Type;

            if(Guild == null && model.GuildId.IsSpecified)
            {
                Guild = await discord.GetGuildAsync(model.GuildId.Value);
            }

            if (User == null)
            {
                if (model.Member.IsSpecified && model.GuildId.IsSpecified)
                {
                    User = RestGuildUser.Create(Discord, Guild, model.Member.Value);
                }
                else
                {
                    User = RestUser.Create(Discord, model.User.Value);
                }
            }

            if(Channel == null && model.ChannelId.IsSpecified)
            {
                Channel = (IRestMessageChannel)await discord.GetChannelAsync(model.ChannelId.Value);
            }
        }

        internal string SerializePayload(object payload)
        {
            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                DiscordRestClient.Serializer.Serialize(writer, payload);

            return json.ToString();
        }

        /// <inheritdoc/>
        public abstract string Defer(bool ephemeral = false, RequestOptions options = null);
        /// <inheritdoc/>
        public abstract Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);
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
        public abstract string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);
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
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
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
        public abstract Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string text = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
            AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

        #region  IDiscordInteraction
        /// <inheritdoc/>
        Task IDiscordInteraction.RespondAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions, RequestOptions options, MessageComponent component, Embed embed)
            => Task.FromResult(Respond(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed));

        Task IDiscordInteraction.DeferAsync(bool ephemeral, RequestOptions options)
            => Task.FromResult(Defer(ephemeral, options));

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.FollowupAsync(string text, Embed[] embeds, bool isTTS, bool ephemeral, AllowedMentions allowedMentions,
            RequestOptions options, MessageComponent component, Embed embed)
            => await FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, component, embed).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.GetOriginalResponseAsync(RequestOptions options)
            => await GetOriginalResponseAsync(options).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IUserMessage> IDiscordInteraction.ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions options)
            => await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
        #endregion
    }
}

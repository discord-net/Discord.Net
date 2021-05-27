using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.InteractionCreated;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an Interaction recieved over the gateway.
    /// </summary>
    public abstract class SocketInteraction : SocketEntity<ulong>, IDiscordInteraction
    {
        /// <summary>
        ///     The <see cref="SocketGuild"/> this interaction was used in.
        /// </summary>
        public SocketGuild Guild
            => Discord.GetGuild(GuildId);

        /// <summary>
        ///     The <see cref="SocketTextChannel"/> this interaction was used in.
        /// </summary>
        public SocketTextChannel Channel
            => Guild.GetTextChannel(ChannelId);

        /// <summary>
        ///     The <see cref="SocketGuildUser"/> who triggered this interaction.
        /// </summary>
        public SocketGuildUser User { get; private set; }

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
        public object Data { get; private set; }

        /// <summary>
        ///     The version of this interaction.
        /// </summary>
        public int Version { get; private set; }

        public DateTimeOffset CreatedAt { get; }

        /// <summary>
        ///     <see langword="true"/> if the token is valid for replying to, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValidToken
            => CheckToken();

        private ulong GuildId { get; set; }
        private ulong ChannelId { get; set; }
        private ulong UserId { get; set; }

        internal SocketInteraction(DiscordSocketClient client, ulong id)
            : base(client, id)
        {
        }

        internal static SocketInteraction Create(DiscordSocketClient client, Model model)
        {
            if (model.Type == InteractionType.ApplicationCommand)
                return SocketSlashCommand.Create(client, model);
            if (model.Type == InteractionType.MessageComponent)
                return SocketMessageComponent.Create(client, model);
            else
                return null;
        }

        internal virtual void Update(Model model)
        {
            this.Data = model.Data.IsSpecified
                ? model.Data.Value
                : null;

            this.GuildId = model.GuildId;
            this.ChannelId = model.ChannelId;
            this.Token = model.Token;
            this.Version = model.Version;
            this.UserId = model.Member.User.Id;
            this.Type = model.Type;

            if (this.User == null)
                this.User = SocketGuildUser.Create(this.Guild, Discord.State, model.Member); // Change from getter.
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

        public virtual Task<RestUserMessage> RespondAsync(string text = null, bool isTTS = false, Embed embed = null, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
            bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        { return null; }

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
        public virtual Task<RestFollowupMessage> FollowupAsync(string text = null, bool isTTS = false, Embed embed = null, bool ephemeral = false,
             InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource,
             AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null)
        { return null; }

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
            return (DateTime.UtcNow - this.CreatedAt.UtcDateTime).TotalMinutes >= 15d;
        }
    }
}

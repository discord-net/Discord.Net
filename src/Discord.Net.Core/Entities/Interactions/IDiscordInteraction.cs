using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a discord interaction
    ///     <para>
    ///         An interaction is the base "thing" that is sent when a user invokes a command, and is the same for Slash Commands
    ///         and other future interaction types. see <see href="https://discord.com/developers/docs/interactions/slash-commands#interaction"/>.
    ///     </para>
    /// </summary>
    public interface IDiscordInteraction : ISnowflakeEntity
    {
        /// <summary>
        ///     The id of the interaction.
        /// </summary>
        new ulong Id { get; }

        /// <summary>
        ///     The type of this <see cref="IDiscordInteraction"/>.
        /// </summary>
        InteractionType Type { get; }

        /// <summary>
        ///     Represents the data sent within this interaction.
        /// </summary>
        IDiscordInteractionData Data { get; }

        /// <summary>
        ///     A continuation token for responding to the interaction.
        /// </summary>
        string Token { get; }

        /// <summary>
        ///     read-only property, always 1.
        /// </summary>
        int Version { get; }

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
        Task RespondAsync (string text = null, Embed[] embeds = null, bool isTTS = false,
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
        Task<IUserMessage> FollowupAsync (string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false,
             AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null);

        /// <summary>
        ///     Gets the original response for this interaction.
        /// </summary>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="IUserMessage"/> that represents the initial response.</returns>
        Task<IUserMessage> GetOriginalResponseAsync (RequestOptions options = null);

        /// <summary>
        ///     Edits original response for this interaction.
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this async request.</param>
        /// <returns>A <see cref="IUserMessage"/> that represents the initial response.</returns>
        Task<IUserMessage> ModifyOriginalResponseAsync (Action<MessageProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Acknowledges this interaction.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation of acknowledging the interaction.
        /// </returns>
        Task DeferAsync (bool ephemeral = false, RequestOptions options = null);
    }
}

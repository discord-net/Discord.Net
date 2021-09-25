using Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.AutocompleteInteractionData;


namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a <see cref="InteractionType.ApplicationCommandAutocomplete"/> received over the gateway.
    /// </summary>
    public class SocketAutocompleteInteraction : SocketInteraction
    {
        /// <summary>
        ///     The autocomplete data of this interaction.
        /// </summary>
        public new SocketAutocompleteInteractionData Data { get; }

        internal SocketAutocompleteInteraction(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
            : base(client, model.Id, channel)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            if (dataModel != null)
                Data = new SocketAutocompleteInteractionData(dataModel);
        }

        internal new static SocketAutocompleteInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            var entity =  new SocketAutocompleteInteraction(client, model, channel);
            entity.Update(model);
            return entity;
        }

        /// <summary>
        ///     Responds to this interaction with a set of choices.
        /// </summary>
        /// <param name="result">
        ///     The set of choices for the user to pick from.
        ///     <remarks>
        ///         A max of 20 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of responding to this interaction.
        /// </returns>
        public Task RespondAsync(IEnumerable<AutocompleteResult> result, RequestOptions options = null)
            => InteractionHelper.SendAutocompleteResult(Discord, result, Id, Token, options);

        /// <summary>
        ///     Responds to this interaction with a set of choices.
        /// </summary>
        /// <param name="options">The request options for this response.</param>
        /// <param name="result">
        ///  The set of choices for the user to pick from.
        ///     <remarks>
        ///         A max of 20 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation of responding to this interaction.
        /// </returns>
        public Task RespondAsync(RequestOptions options = null, params AutocompleteResult[] result)
            => InteractionHelper.SendAutocompleteResult(Discord, result, Id, Token, options);


        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot be defered!", true)]
        public override Task DeferAsync(bool ephemeral = false, RequestOptions options = null) => throw new NotSupportedException();

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string text = null, Stream fileStream = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string text = null, string filePath = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have normal responses!", true)]
        public override Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null) => throw new NotSupportedException();
    }
}

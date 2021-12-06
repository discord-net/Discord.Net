using Discord.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.AutocompleteInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a <see cref="InteractionType.ApplicationCommandAutocomplete"/> received over the gateway.
    /// </summary>
    public class SocketAutocompleteInteraction : SocketInteraction, IAutocompleteInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     The autocomplete data of this interaction.
        /// </summary>
        public new SocketAutocompleteInteractionData Data { get; }

        public override bool HasResponded { get; internal set; }
        private object _lock = new object();

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
            var entity = new SocketAutocompleteInteraction(client, model, channel);
            entity.Update(model);
            return entity;
        }

        /// <summary>
        ///     Responds to this interaction with a set of choices.
        /// </summary>
        /// <param name="result">
        ///     The set of choices for the user to pick from.
        ///     <remarks>
        ///         A max of 25 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <param name="options">The request options for this response.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of responding to this interaction.
        /// </returns>
        public async Task RespondAsync(IEnumerable<AutocompleteResult> result, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond twice to the same interaction");
                }
            }

            await InteractionHelper.SendAutocompleteResultAsync(Discord, result, Id, Token, options).ConfigureAwait(false);
            lock (_lock)
            {
                HasResponded = true;
            }
        }

        /// <summary>
        ///     Responds to this interaction with a set of choices.
        /// </summary>
        /// <param name="options">The request options for this response.</param>
        /// <param name="result">
        ///  The set of choices for the user to pick from.
        ///     <remarks>
        ///         A max of 25 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation of responding to this interaction.
        /// </returns>
        public Task RespondAsync(RequestOptions options = null, params AutocompleteResult[] result)
            => RespondAsync(result, options);
        public override Task<RestInteractionMessage> RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string fileName = null, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task<RestFollowupMessage> FollowupWithFileAsync(FileAttachment attachment, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task<RestFollowupMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");

        //IAutocompleteInteraction
        /// <inheritdoc/>
        IAutocompleteInteractionData IAutocompleteInteraction.Data => Data;

        //IDiscordInteraction
        /// <inheritdoc/>
        IDiscordInteractionData IDiscordInteraction.Data => Data;
    }
}

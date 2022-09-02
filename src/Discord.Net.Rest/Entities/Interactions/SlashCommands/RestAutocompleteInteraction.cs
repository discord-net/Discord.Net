using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Interaction;
using DataModel = Discord.API.AutocompleteInteractionData;
using System.IO;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based autocomplete interaction.
    /// </summary>
    public class RestAutocompleteInteraction : RestInteraction, IAutocompleteInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the autocomplete data of this interaction.
        /// </summary>
        public new RestAutocompleteInteractionData Data { get; }

        private object _lock = new object();

        internal RestAutocompleteInteraction(DiscordRestClient client, Model model)
            : base(client, model.Id)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            if (dataModel != null)
                Data = new RestAutocompleteInteractionData(dataModel);
        }

        internal new static async Task<RestAutocompleteInteraction> CreateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            var entity = new RestAutocompleteInteraction(client, model);
            await entity.UpdateAsync(client, model, doApiCall).ConfigureAwait(false);
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
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public string Respond(IEnumerable<AutocompleteResult> result, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            lock (_lock)
            {
                if (HasResponded)
                {
                    throw new InvalidOperationException("Cannot respond twice to the same interaction");
                }

                HasResponded = true;
            }

            var model = new API.InteractionResponse
            {
                Type = InteractionResponseType.ApplicationCommandAutocompleteResult,
                Data = new API.InteractionCallbackData
                {
                    Choices = result.Any()
                        ? result.Select(x => new API.ApplicationCommandOptionChoice { Name = x.Name, Value = x.Value }).ToArray()
                        : Array.Empty<API.ApplicationCommandOptionChoice>()
                }
            };

            return SerializePayload(model);
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
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public string Respond(RequestOptions options = null, params AutocompleteResult[] result)
            => Respond(result, options);
        public override string Defer(bool ephemeral = false, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");
        public override string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
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
        public override string RespondWithModal(Modal modal, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions don't support this method!");

        //IAutocompleteInteraction
        /// <inheritdoc/>
        IAutocompleteInteractionData IAutocompleteInteraction.Data => Data;
    }
}

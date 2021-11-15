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
    public class RestAutocompleteInteraction : RestInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the autocomplete data of this interaction.
        /// </summary>
        public new RestAutocompleteInteractionData Data { get; }

        internal override bool _hasResponded { get; set; }
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

        internal new static async Task<RestAutocompleteInteraction> CreateAsync(DiscordRestClient client, Model model)
        {
            var entity = new RestAutocompleteInteraction(client, model);
            await entity.UpdateAsync(client, model).ConfigureAwait(false);
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
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public string Respond(IEnumerable<AutocompleteResult> result, RequestOptions options = null)
        {
            if (!InteractionHelper.CanSendResponse(this))
                throw new TimeoutException($"Cannot respond to an interaction after {InteractionHelper.ResponseTimeLimit} seconds!");

            lock (_lock)
            {
                if (_hasResponded)
                {
                    throw new InvalidOperationException("Cannot respond twice to the same interaction");
                }
            }

            lock (_lock)
            {
                _hasResponded = true;
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
        ///         A max of 20 choices are allowed. Passing <see langword="null"/> for this argument will show the executing user that
        ///         there is no choices for their autocompleted input.
        ///     </remarks>
        /// </param>
        /// <returns>
        ///     A string that contains json to write back to the incoming http request.
        /// </returns>
        public string Respond(RequestOptions options = null, params AutocompleteResult[] result)
            => Respond(result, options);

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot be deferred!", true)]
        public override string Defer(bool ephemeral = false, RequestOptions options = null)
            => throw new NotSupportedException("Autocomplete interactions cannot be deferred!");

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null)
            => throw new NotSupportedException("Autocomplete interactions cannot be deferred!");

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupWithFileAsync(Stream fileStream, string fileName, string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null)
            => throw new NotSupportedException("Autocomplete interactions cannot be deferred!");

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have followups!", true)]
        public override Task<RestFollowupMessage> FollowupWithFileAsync(string filePath, string text = null, string fileName = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null)
            => throw new NotSupportedException("Autocomplete interactions cannot be deferred!");

        /// <inheritdoc/>
        [Obsolete("Autocomplete interactions cannot have normal responses!", true)]
        public override string Respond(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent component = null, Embed embed = null)
            => throw new NotSupportedException("Autocomplete interactions cannot be deferred!");

        IDiscordInteractionData IDiscordInteraction.Data => Data;

    }
}

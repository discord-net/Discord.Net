using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataModel = Discord.API.AutocompleteInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents data for a slash commands autocomplete interaction.
    /// </summary>
    public class SocketAutocompleteInteractionData : IAutocompleteInteractionData, IDiscordInteractionData
    {
        /// <inheritdoc/>
        public string CommandName { get; }

        /// <inheritdoc/>
        public ulong CommandId { get; }

        /// <inheritdoc/>
        public ApplicationCommandType Type { get; }

        /// <inheritdoc/>
        public ulong Version { get; }

        /// <inheritdoc/>
        public AutocompleteOption Current { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<AutocompleteOption> Options { get; }

        internal SocketAutocompleteInteractionData(DataModel model)
        {
            var options = model.Options.SelectMany(GetOptions);

            Current = options.FirstOrDefault(x => x.Focused);
            Options = options.ToImmutableArray();

            if (Options.Count == 1 && Current == null)
                Current = Options.FirstOrDefault();

            CommandName = model.Name;
            CommandId = model.Id;
            Type = model.Type;
            Version = model.Version;
        }

        private List<AutocompleteOption> GetOptions(API.AutocompleteInteractionDataOption model)
        {
            var options = new List<AutocompleteOption>();

            options.Add(new AutocompleteOption(model.Type, model.Name, model.Value.GetValueOrDefault(null), model.Focused.GetValueOrDefault(false)));

            if (model.Options.IsSpecified)
            {
                options.AddRange(model.Options.Value.SelectMany(GetOptions));
            }

            return options;
        }
    }
}

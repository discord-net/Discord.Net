using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = Discord.API.AutocompleteInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the data for a <see cref="RestAutocompleteInteraction"/>.
    /// </summary>
    public class RestAutocompleteInteractionData : IAutocompleteInteractionData
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

        internal RestAutocompleteInteractionData(DataModel model)
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

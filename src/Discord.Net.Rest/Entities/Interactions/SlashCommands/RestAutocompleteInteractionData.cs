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
        /// <summary>
        ///     Gets the name of the invoked command.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        ///     Gets the id of the invoked command.
        /// </summary>
        public ulong CommandId { get; }

        /// <summary>
        ///     Gets the type of the invoked command.
        /// </summary>
        public ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets the version of the invoked command.
        /// </summary>
        public ulong Version { get; }

        /// <summary>
        ///     Gets the current autocomplete option that is actively being filled out.
        /// </summary>
        public AutocompleteOption Current { get; }

        /// <summary>
        ///     Gets a collection of all the other options the executing users has filled out.
        /// </summary>
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

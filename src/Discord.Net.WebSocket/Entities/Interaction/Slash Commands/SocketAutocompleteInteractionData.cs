using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = Discord.API.AutocompleteInteractionData;


namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents data for a slash commands autocomplete interaction.
    /// </summary>
    public class SocketAutocompleteInteractionData
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

        internal SocketAutocompleteInteractionData(DataModel model)
        {
            var options = model.Options.SelectMany(x => GetOptions(x));

            Current = options.FirstOrDefault(x => x.Focused);
            Options = options.ToImmutableArray();

            if (options != null && options.Count() == 1 && Current == null)
                Current = Options.FirstOrDefault();

            CommandName = model.Name;
            CommandId = model.Id;
            Type = model.Type;
            Version = model.Version;
        }

        private List<AutocompleteOption> GetOptions(API.AutocompleteInteractionDataOption model)
        {
            List<AutocompleteOption> options = new List<AutocompleteOption>();

            if (model.Options.IsSpecified)
            {
                options.AddRange(model.Options.Value.SelectMany(x => GetOptions(x)));
            }
            else if(model.Focused.IsSpecified)
            {
                options.Add(new AutocompleteOption(model.Type, model.Name, model.Value.GetValueOrDefault(null), model.Focused.Value));
            }

            return options;
        }
    }
}

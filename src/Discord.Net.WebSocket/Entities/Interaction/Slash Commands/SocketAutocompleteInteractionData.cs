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
        ///     Gets the current autocomplete option that is activly being filled out.
        /// </summary>
        public AutocompleteOption Current { get; }

        /// <summary>
        ///     Gets a collection of all the other options the executing users has filled out.
        /// </summary>
        public IReadOnlyCollection<AutocompleteOption> Options { get; }

        internal SocketAutocompleteInteractionData(DataModel model)
        {
            var options = model.Options.Select(x => new AutocompleteOption(x.Type, x.Name, x.Value, x.Focused));

            Current = options.FirstOrDefault(x => x.Focused);
            Options = options.ToImmutableArray();

            CommandName = model.Name;
            CommandId = model.Id;
            Type = model.Type;
            Version = model.Version;
        }
    }
}

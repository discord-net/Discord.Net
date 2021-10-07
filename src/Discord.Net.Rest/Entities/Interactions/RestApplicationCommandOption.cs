using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IApplicationCommandOption"/>.
    /// </summary>
    public class RestApplicationCommandOption : IApplicationCommandOption
    {
        #region RestApplicationCommandOption
        /// <inheritdoc/>
        public ApplicationCommandOptionType Type { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool? IsDefault { get; private set; }

        /// <inheritdoc/>
        public bool? IsRequired { get; private set; }

        /// <summary>
        ///     A collection of <see cref="RestApplicationCommandChoice"/>'s for this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandChoice> Choices { get; private set; }

        /// <summary>
        ///     A collection of <see cref="RestApplicationCommandOption"/>'s for this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

        internal RestApplicationCommandOption() { }

        internal static RestApplicationCommandOption Create(Model model)
        {
            var options = new RestApplicationCommandOption();
            options.Update(model);
            return options;
        }

        internal void Update(Model model)
        {
            Type = model.Type;
            Name = model.Name;
            Description = model.Description;

            if (model.Default.IsSpecified)
                IsDefault = model.Default.Value;

            if (model.Required.IsSpecified)
                IsRequired = model.Required.Value;

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => Create(x)).ToImmutableArray()
                : null;

            Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(x => new RestApplicationCommandChoice(x)).ToImmutableArray()
                : null;
        }
        #endregion

        #region IApplicationCommandOption
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options
            => Options;
        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices
            => Choices;
        #endregion
    }
}

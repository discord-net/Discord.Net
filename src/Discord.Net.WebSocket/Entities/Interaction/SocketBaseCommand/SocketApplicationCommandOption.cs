using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents an option for a <see cref="SocketApplicationCommand"/>.
    /// </summary>
    public class SocketApplicationCommandOption : IApplicationCommandOption
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public ApplicationCommandOptionType Type { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool? Default { get; private set; }

        /// <inheritdoc/>
        public bool? Required { get; private set; }

        /// <summary>
        ///     Choices for string and int types for the user to pick from.
        /// </summary>
        public IReadOnlyCollection<SocketApplicationCommandChoice> Choices { get; private set; }

        /// <summary>
        ///     If the option is a subcommand or subcommand group type, this nested options will be the parameters.
        /// </summary>
        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        /// <summary>
        ///     The allowed channel types for this option.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; private set; }

        internal SocketApplicationCommandOption() { }
        internal static SocketApplicationCommandOption Create(Model model)
        {
            var entity = new SocketApplicationCommandOption();
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            Name = model.Name;
            Type = model.Type;
            Description = model.Description;

            Default = model.Default.IsSpecified
                ? model.Default.Value
                : null;

            Required = model.Required.IsSpecified
                ? model.Required.Value
                : null;

            Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(x => SocketApplicationCommandChoice.Create(x)).ToImmutableArray()
                : new ImmutableArray<SocketApplicationCommandChoice>();

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => SocketApplicationCommandOption.Create(x)).ToImmutableArray()
                : new ImmutableArray<SocketApplicationCommandOption>();

            ChannelTypes = model.ChannelTypes.IsSpecified
                ? model.ChannelTypes.Value.ToImmutableArray()
                : new ImmutableArray<ChannelType>();
        }

        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices => Choices;
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options => Options;
    }
}

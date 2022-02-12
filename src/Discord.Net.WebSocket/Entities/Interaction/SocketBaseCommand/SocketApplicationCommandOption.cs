using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        public bool? IsDefault { get; private set; }

        /// <inheritdoc/>
        public bool? IsRequired { get; private set; }

        public bool? IsAutocomplete { get; private set; }

        /// <inheritdoc/>
        public double? MinValue { get; private set; }

        /// <inheritdoc/>
        public double? MaxValue { get; private set; }

        /// <summary>
        ///     Gets a collection of choices for the user to pick from.
        /// </summary>
        public IReadOnlyCollection<SocketApplicationCommandChoice> Choices { get; private set; }

        /// <summary>
        ///     Gets a collection of nested options.
        /// </summary>
        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        /// <summary>
        ///     Gets the allowed channel types for this option.
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

            IsDefault = model.Default.ToNullable();

            IsRequired = model.Required.ToNullable();

            MinValue = model.MinValue.ToNullable();

            MaxValue = model.MaxValue.ToNullable();

            IsAutocomplete = model.Autocomplete.ToNullable();

            Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(SocketApplicationCommandChoice.Create).ToImmutableArray()
                : ImmutableArray.Create<SocketApplicationCommandChoice>();

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(Create).ToImmutableArray()
                : ImmutableArray.Create<SocketApplicationCommandOption>();

            ChannelTypes = model.ChannelTypes.IsSpecified
                ? model.ChannelTypes.Value.ToImmutableArray()
                : ImmutableArray.Create<ChannelType>();
        }

        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices => Choices;
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options => Options;
    }
}

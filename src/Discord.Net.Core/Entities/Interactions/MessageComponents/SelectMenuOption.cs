namespace Discord
{
    /// <summary>
    ///     Represents a choice for a <see cref="SelectMenuComponent"/>.
    /// </summary>
    public class SelectMenuOption
    {
        /// <summary>
        ///     Gets the user-facing name of the option.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets the dev-define value of the option.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Gets a description of the option.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets the <see cref="IEmote"/> displayed with this menu option.
        /// </summary>
        public IEmote Emote { get; }

        /// <summary>
        ///     Gets whether or not this option will render as selected by default.
        /// </summary>
        public bool? IsDefault { get; }

        /// <summary>
        ///     Gets whether or not this option is disabled.
        /// </summary>
        public bool? Disabled { get; }

        internal SelectMenuOption(string label, string value, string description, IEmote emote, bool? defaultValue, bool? disabled = null)
        {
            Label = label;
            Value = value;
            Description = description;
            Emote = emote;
            IsDefault = defaultValue;
            Disabled = disabled;
        }
    }
}

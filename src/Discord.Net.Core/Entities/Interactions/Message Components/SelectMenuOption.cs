namespace Discord
{
    /// <summary>
    ///     Represents a choice for a <see cref="SelectMenuComponent"/>.
    /// </summary>
    public class SelectMenuOption
    {
        /// <summary>
        ///     The user-facing name of the option, max 25 characters.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     The dev-define value of the option, max 100 characters.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     An additional description of the option, max 50 characters.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     A <see cref="IEmote"/> that will be displayed with this menu option.
        /// </summary>
        public IEmote Emote { get; }

        /// <summary>
        ///     Will render this option as selected by default.
        /// </summary>
        public bool? Default { get; }

        internal SelectMenuOption(string label, string value, string description, IEmote emote, bool? defaultValue)
        {
            Label = label;
            Value = value;
            Description = description;
            Emote = emote;
            Default = defaultValue;
        }
    }
}

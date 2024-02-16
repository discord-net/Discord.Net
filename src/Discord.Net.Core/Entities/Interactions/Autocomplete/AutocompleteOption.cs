namespace Discord
{
    /// <summary>
    ///     Represents an autocomplete option.
    /// </summary>
    public class AutocompleteOption
    {
        /// <summary>
        ///     Gets the type of this option.
        /// </summary>
        public ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     Gets the name of the option.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the value of the option.
        /// </summary>
        public object Value { get; }

        /// <summary>
        ///     Gets whether or not this option is focused by the executing user.
        /// </summary>
        public bool Focused { get; }

        internal AutocompleteOption(ApplicationCommandOptionType type, string name, object value, bool focused)
        {
            Type = type;
            Name = name;
            Value = value;
            Focused = focused;
        }
    }
}

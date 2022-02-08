namespace Discord
{
    /// <summary>
    ///     Represents a type of a component.
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        ///     A container for other components.
        /// </summary>
        ActionRow = 1,

        /// <summary>
        ///     A clickable button.
        /// </summary>
        Button = 2,

        /// <summary>
        ///     A select menu for picking from choices.
        /// </summary>
        SelectMenu = 3,

        /// <summary>
        ///     A box for entering text.
        /// </summary>
        TextInput = 4,

        /// <summary>
        ///     An interaction sent when a model is submitted.
        /// </summary>
        ModalSubmit = 5,
    }
}

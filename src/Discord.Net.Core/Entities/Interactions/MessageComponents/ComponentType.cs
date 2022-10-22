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
        ///     A select menu for picking from users.
        /// </summary>
        UserSelect = 5,

        /// <summary>
        ///     A select menu for picking from roles.
        /// </summary>
        RoleSelect = 6,

        /// <summary>
        ///     A select menu for picking from roles and users.
        /// </summary>
        MentionableSelect = 7,

        /// <summary>
        ///     A select menu for picking from channels.
        /// </summary>
        ChannelSelect = 8,
    }
}

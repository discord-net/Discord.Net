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

        /// <summary>
        ///     A container to display text alongside an accessory component.
        /// </summary>
        Section = 9,

        /// <summary>
        ///     A component displaying Markdown text.
        /// </summary>
        TextDisplay = 10,

        /// <summary>
        ///     A small image that can be used as an accessory.
        /// </summary>
        Thumbnail = 11,

        /// <summary>
        ///     A component displaying images and other media.
        /// </summary>
        MediaGallery = 12,

        /// <summary>
        ///     A component displaying an attached file.
        /// </summary>
        File = 13,

        /// <summary>
        ///     A component to add vertical padding between other components.
        /// </summary>
        Separator = 14,

        /// <summary>
        ///     A container that visually groups a set of components.
        /// </summary>
        Container = 17,

        /// <summary>
        ///     
        /// </summary>
        Label = 18,
    }
}

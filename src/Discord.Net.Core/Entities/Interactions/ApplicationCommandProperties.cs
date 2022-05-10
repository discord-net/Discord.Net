namespace Discord
{
    /// <summary>
    ///     Represents the base class to create/modify application commands.
    /// </summary>
    public abstract class ApplicationCommandProperties
    {
        internal abstract ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets or sets the name of this command.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild. Default is <see langword="true"/>
        /// </summary>
        public Optional<bool> IsDefaultPermission { get; set; }

        /// <summary>
        ///     Gets or sets whether or not this command can be used in DMs.
        /// </summary>
        public Optional<bool> IsDMEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the default permissions required by a user to execute this application command.
        /// </summary>
        public Optional<GuildPermission> DefaultMemberPermissions { get; set; }

        internal ApplicationCommandProperties() { }
    }
}

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="IGuild" /> with the specified changes.
    /// </summary>
    /// <see cref="IGuild.ModifyAsync"/>
    public class GuildProperties
    {
        /// <summary>
        ///     Gets or sets the name of the guild. Must be within 100 characters.
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Gets or sets the region for the guild's voice connections.
        /// </summary>
        public Optional<IVoiceRegion> Region { get; set; }
        /// <summary>
        ///     Gets or sets the ID of the region for the guild's voice connections.
        /// </summary>
        public Optional<string> RegionId { get; set; }
        /// <summary>
        ///     Gets or sets the verification level new users need to achieve before speaking.
        /// </summary>
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        /// <summary>
        ///     Gets or sets the default message notification state for the guild.
        /// </summary>
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        /// <summary>
        ///     Gets or sets how many seconds before a user is sent to AFK. This value MUST be one of: (60, 300, 900,
        ///     1800, 3600).
        /// </summary>
        public Optional<int> AfkTimeout { get; set; }
        /// <summary>
        ///     Gets or sets the icon of the guild.
        /// </summary>
        public Optional<Image?> Icon { get; set; }
        /// <summary>
        ///     Gets or sets the guild's splash image.
        /// </summary>
        /// <remarks>
        ///     The guild must be partnered for this value to have any effect.
        /// </remarks>
        public Optional<Image?> Splash { get; set; }
        /// <summary>
        ///     Gets or sets the <see cref="IVoiceChannel"/> where AFK users should be sent.
        /// </summary>
        public Optional<IVoiceChannel> AfkChannel { get; set; }
        /// <summary>
        ///     Gets or sets the ID of the <see cref="IVoiceChannel"/> where AFK users should be sent.
        /// </summary>
        public Optional<ulong?> AfkChannelId { get; set; }
        /// <summary>
        ///     Gets or sets the <see cref="ITextChannel" /> where system messages should be sent.
        /// </summary>
        public Optional<ITextChannel> SystemChannel { get; set; }
        /// <summary>
        ///     Gets or sets the ID of the <see cref="ITextChannel" /> where system messages should be sent.
        /// </summary>
        public Optional<ulong?> SystemChannelId { get; set; }
        /// <summary>
        ///     Gets or sets the owner of this guild.
        /// </summary>
        public Optional<IUser> Owner { get; set; }
        /// <summary>
        ///     Gets or sets the ID of the owner of this guild.
        /// </summary>
        public Optional<ulong> OwnerId { get; set; }
        /// <summary>
        ///     Gets or sets the explicit content filter level of this guild.
        /// </summary>
        public Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; set; }
    }
}

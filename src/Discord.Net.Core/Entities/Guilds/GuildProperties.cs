namespace Discord
{
    /// <summary>
    /// Modify an IGuild with the specified changes
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await Context.Guild.ModifyAsync(async x =>
    /// {
    ///     x.Name = "aaaaaah";
    ///     x.RegionId = (await Context.Client.GetOptimalVoiceRegionAsync()).Id;
    /// });
    /// </code>
    /// </example>
    /// <see cref="IGuild"/>
    public class GuildProperties
    {
        public Optional<string> Username { get; set; }
        /// <summary>
        /// The name of the Guild
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        /// The region for the Guild's voice connections
        /// </summary>
        public Optional<IVoiceRegion> Region { get; set; }
        /// <summary>
        /// The ID of the region for the Guild's voice connections
        /// </summary>
        public Optional<string> RegionId { get; set; }
        /// <summary>
        /// What verification level new users need to achieve before speaking
        /// </summary>
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        /// <summary>
        /// The default message notification state for the guild
        /// </summary>
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        /// <summary>
        /// How many seconds before a user is sent to AFK. This value MUST be one of: (60, 300, 900, 1800, 3600).
        /// </summary>
        public Optional<int> AfkTimeout { get; set; }
        /// <summary>
        /// The icon of the guild
        /// </summary>
        public Optional<Image?> Icon { get; set; }
        /// <summary>
        /// The guild's splash image
        /// </summary>
        /// <remarks>
        /// The guild must be partnered for this value to have any effect.
        /// </remarks>
        public Optional<Image?> Splash { get; set; }
        /// <summary>
        /// The IVoiceChannel where AFK users should be sent.
        /// </summary>
        public Optional<IVoiceChannel> AfkChannel { get; set; }
        /// <summary>
        /// The ID of the IVoiceChannel where AFK users should be sent.
        /// </summary>
        public Optional<ulong?> AfkChannelId { get; set; }
        /// <summary>
        /// The owner of this guild.
        /// </summary>
        public Optional<IUser> Owner { get; set; }
        /// <summary>
        /// The ID of the owner of this guild.
        /// </summary>
        public Optional<ulong> OwnerId { get; set; }
    }
}

namespace Discord.Rest
{
    /// <summary>
    ///     Represents information for a guild.
    /// </summary>
    public struct GuildInfo
    {
        internal GuildInfo(int? afkTimeout, DefaultMessageNotifications? defaultNotifs,
            ulong? afkChannel, string name, string region, string icon,
            VerificationLevel? verification, IUser owner, MfaLevel? mfa, int? filter)
        {
            AfkTimeout = afkTimeout;
            DefaultMessageNotifications = defaultNotifs;
            AfkChannelId = afkChannel;
            Name = name;
            RegionId = region;
            IconHash = icon;
            VerificationLevel = verification;
            Owner = owner;
            MfaLevel = mfa;
            ContentFilterLevel = filter;
        }

        /// <summary>
        ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
        ///     automatically moved to the AFK voice channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the amount of time in seconds for a user to be marked as inactive
        ///     and moved into the AFK voice channel.
        /// </returns>
        public int? AfkTimeout { get; }
        /// <summary>
        ///     Gets the default message notifications for users who haven't explicitly set their notification settings.
        /// </summary>
        public DefaultMessageNotifications? DefaultMessageNotifications { get; }
        /// <summary>
        ///     Gets the ID of the AFK voice channel for this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the AFK voice channel; <c>null</c> if
        ///     none is set.
        /// </returns>
        public ulong? AfkChannelId { get; }
        /// <summary>
        ///     Gets the name of this guild.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this guild.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the ID of the region hosting this guild's voice channels.
        /// </summary>
        public string RegionId { get; }
        /// <summary>
        ///     Gets the ID of this guild's icon.
        /// </summary>
        /// <returns>
        ///     A string containing the identifier for the splash image; <c>null</c> if none is set.
        /// </returns>
        public string IconHash { get; }
        /// <summary>
        ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
        /// </summary>
        /// <returns>
        ///     The level of requirements.
        /// </returns>
        public VerificationLevel? VerificationLevel { get; }
        /// <summary>
        ///     Gets the owner of this guild.
        /// </summary>
        /// <returns>
        ///     A user object representing the owner of this guild.
        /// </returns>
        public IUser Owner { get; }
        /// <summary>
        ///     Gets the level of Multi-Factor Authentication requirements a user must fulfill before being allowed to
        ///     perform administrative actions in this guild.
        /// </summary>
        /// <returns>
        ///     The level of MFA requirement.
        /// </returns>
        public MfaLevel? MfaLevel { get; }
        public int? ContentFilterLevel { get; }
    }
}

using System.Globalization;

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
        ///     Gets or sets the banner of the guild.
        /// </summary>
        public Optional<Image?> Banner { get; set; }
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
        /// <summary>
        ///     Gets or sets the flags that DISABLE types of system channel messages.
        /// </summary>
        /// <remarks>
        ///     These flags are inverted. Setting a flag will disable that system channel message from being sent.
        ///     A value of <see cref="SystemChannelMessageDeny.None"/> will allow all system channel message types to be sent,
        ///     given that the <see cref="SystemChannelId"/> has also been set.
        ///     A value of <see cref="SystemChannelMessageDeny.GuildBoost"/> will deny guild boost messages from being sent, and allow all
        ///     other types of messages.
        ///     Refer to the extension methods <see cref="GuildExtensions.GetGuildBoostMessagesEnabled(IGuild)"/>,  
        ///     <see cref="GuildExtensions.GetWelcomeMessagesEnabled(IGuild)"/>, <see cref="GuildExtensions.GetGuildSetupTipMessagesEnabled(IGuild)"/>,
        ///     and <see cref="GuildExtensions.GetGuildWelcomeMessageReplyEnabled(IGuild)"/> to check if these system channel message types
        ///     are enabled, without the need to manipulate the logic of the flag.
        /// </remarks>
        public Optional<SystemChannelMessageDeny> SystemChannelFlags { get; set; }
        /// <summary>
        ///     Gets or sets the preferred locale of the guild in IETF BCP 47 language tag format.
        /// </summary>
        /// <remarks>
        ///     This property takes precedence over <see cref="PreferredCulture"/>.
        ///     When it is set, the value of <see cref="PreferredCulture"/>
        ///     will not be used.
        /// </remarks>
        public Optional<string> PreferredLocale { get; set; }
        /// <summary>
        ///     Gets or sets the preferred locale of the guild.
        /// </summary>
        /// <remarks>
        ///     The <see cref="PreferredLocale"/> property takes precedence
        ///     over this property. When <see cref="PreferredLocale"/> is set,
        ///     the value of <see cref="PreferredCulture"/> will be unused.
        /// </remarks>
        public Optional<CultureInfo> PreferredCulture { get; set; }
        /// <summary>
        ///     Gets or sets if the boost progress bar is enabled.
        /// </summary>
        public Optional<bool> IsBoostProgressBarEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the guild features enabled in this guild. Features that are not mutable will be ignored.
        /// </summary>
        public Optional<GuildFeature> Features { get; set; }
    }
}

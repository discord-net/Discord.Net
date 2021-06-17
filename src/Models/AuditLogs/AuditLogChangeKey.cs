namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the audit log change key for an <see cref="AuditLogChange"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-change-object-audit-log-change-key"/>
    /// </remarks>
    public enum AuditLogChangeKey
    {
        //
        // Any
        //

        /// <summary>
        /// The id of the changed entity - sometimes used in conjunction with other keys.
        /// </summary>
        Id,

        /// <summary>
        /// Type of entity created.
        /// </summary>
        Type,

        //
        // Guild
        //

        /// <summary>
        /// Name changed.
        /// </summary> 
        Name,

        /// <summary>
        /// Icon changed.
        /// </summary>
        IconHash,

        /// <summary>
        /// Invite splash page artwork changed.
        /// </summary>
        SplashHash,

        /// <summary>
        /// Discovery splash changed.
        /// </summary>
        DiscoverySplashHash,

        /// <summary>
        /// Guild banner changed.
        /// </summary>
        BannerHash,

        /// <summary>
        /// Owner changed.
        /// </summary>
        OwnerId,

        /// <summary>
        /// <see cref="VoiceRegion"/> changed.
        /// </summary>
        Region,

        /// <summary>
        /// Preferred locale changed.
        /// </summary>
        PreferredLocale,

        /// <summary>
        /// Id of the afk <see cref="Channel"/> changed.
        /// </summary>
        AfkChannelId,

        /// <summary>
        /// Afk timeout duration changed.
        /// </summary>
        AfkTimeout,

        /// <summary>
        /// Id of the rules <see cref="Channel"/> changed.
        /// </summary>
        RulesChannelId,

        /// <summary>
        /// Id of the public updates <see cref="Channel"/> changed.
        /// </summary>
        PublicUpdatesChannelId,

        /// <summary>
        /// Two-factor auth requirement (<see cref="Models.MfaLevel"/>) changed.
        /// </summary>
        MfaLevel,

        /// <summary>
        /// Required <see cref="Models.VerificationLevel"/> changed.
        /// </summary>
        VerificationLevel,

        /// <summary>
        /// Change in the <see cref="ExplicitContentFilterLevel"/>, that changes whose
        /// <see cref="Message"/>s are scanned and delete for explicit content in the <see cref="Guild"/>.
        /// </summary>
        ExplicitContentFilter,

        /// <summary>
        /// Default <see cref="DefaultMessageNotificationLevel"/> changed.
        /// </summary>
        DefaultMessageNotifications,

        /// <summary>
        /// <see cref="Guild"/> <see cref="Invite"/> vanity url changed.
        /// </summary>
        VanityUrlCode,

        /// <summary>
        /// New <see cref="Role"/> added.
        /// </summary>
        Add, //TODO: This is actually called $add

        /// <summary>
        /// <see cref="Role"/> removed.
        /// </summary>
        Remove, //TODO: This is actually called $remove

        /// <summary>
        /// Change in number of days after which inactive and <see cref="Role"/>-unassigned <see cref="User"/>s are kicked.
        /// </summary>
        PruneDeleteDays,

        /// <summary>
        /// <see cref="GuildWidget"/> enabled/disabled.
        /// </summary>
        WidgetEnabled,

        /// <summary>
        /// <see cref="Channel"/> Id of <see cref="GuildWidget"/> changed.
        /// </summary>
        WidgetChannelId,

        /// <summary>
        /// Id of the system <see cref="Channel"/> changed.
        /// </summary>
        SystemChannelId,

        //
        // Channel
        //

        /// <summary>
        /// <see cref="GuildTextChannel"/> or <see cref="VoiceChannel"/> position changed.
        /// </summary>
        Position,

        /// <summary>
        /// <see cref="GuildTextChannel"/> topic changed.
        /// </summary>
        Topic,

        /// <summary>
        /// <see cref="VoiceChannel"/> bitrate changed.
        /// </summary>
        Bitrate,

        /// <summary>
        /// <see cref="Overwrite"/>s on a channel changed.
        /// </summary>
        PermissionOverwrites,

        /// <summary>
        /// <see cref="Channel"/> nsfw restriction changed.
        /// </summary>
        Nsfw,

        /// <summary>
        /// <see cref="Application" /> Id of the added or removed <see cref="Webhook"/> or bot.
        /// </summary>
        ApplicationId,

        //
        // Role
        //

        /// <summary>
        /// <see cref="Models.Permissions"/> for a <see cref="Role"/> changed.
        /// </summary>
        Permissions,

        /// <summary>
        /// <see cref="Role"/> color changed.
        /// </summary>
        Color,

        /// <summary>
        /// <see cref="Role"/> is now displayed/no longer displayed separate from other online <see cref="User"/>s.
        /// </summary>
        Hoist,

        /// <summary>
        /// <see cref="Role"/> is now mentionable/unmentionable.
        /// </summary>
        Mentionable,

        /// <summary>
        /// A <see cref="Models.Permissions"/> on a <see cref="Channel"/> was allowed for a <see cref="Role"/>.
        /// </summary>
        Allow,

        /// <summary>
        /// A <see cref="Models.Permissions"/> on a <see cref="Channel"/> was denied for a <see cref="Role"/>.
        /// </summary>
        Deny,

        //
        // Invite
        //

        /// <summary>
        /// <see cref="Invite"/> code changed.
        /// </summary>
        Code,

        /// <summary>
        /// <see cref="Channel"/> for <see cref="Invite"/> code changed.
        /// </summary>
        ChannelId,

        /// <summary>
        /// <see cref="User"/> who created <see cref="Invite"/> code changed.
        /// </summary>
        InviterId,

        /// <summary>
        /// Change to the max number of times <see cref="Invite"/> code can be used.
        /// </summary>
        MaxUses,

        /// <summary>
        /// Number of times <see cref="Invite"/> code used changed.
        /// </summary>
        Uses,

        /// <summary>
        /// How long <see cref="Invite"/> code lasts changed.
        /// </summary>
        MaxAge,

        /// <summary>
        /// <see cref="Invite"/> code is temporary/never expires.
        /// </summary>
        Temporary,

        //
        // User 
        //

        /// <summary>
        /// <see cref="User"/> server deafened/undeafened.
        /// </summary>
        Deaf,

        /// <summary>
        /// <see cref="User"/> server muted/unmuted.
        /// </summary>
        Mute,

        /// <summary>
        /// <see cref="User"/> nickname changed.
        /// </summary>
        Nick,

        /// <summary>
        /// <see cref="User"/> avatar changed.
        /// </summary>
        AvatarHash,

        //
        // Integration
        //

        /// <summary>
        /// <see cref="Integration.EnableEmoticons"/> enabled/disabled.
        /// </summary>
        EnableEmoticons,

        /// <summary>
        /// <see cref="Integration.ExpireBehavior"/> changed.
        /// </summary>
        ExpireBehavior,

        /// <summary>
        /// <see cref="Integration.ExpireGracePeriod"/> changed.
        /// </summary>
        ExpireGracePeriod,

        //
        // Voice channel
        //

        /// <summary>
        /// New user limit in a <see cref="VoiceChannel"/>.
        /// </summary>
        UserLimit,
    }
}

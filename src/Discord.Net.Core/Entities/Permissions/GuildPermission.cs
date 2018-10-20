using System;

namespace Discord
{
    /// <summary> Defines the available permissions for a channel. </summary>
    [Flags]
    public enum GuildPermission : ulong
    {
        // General
        /// <summary>
        ///     Allows creation of instant invites.
        /// </summary>
        CreateInstantInvite = 0x00_00_00_01,
        /// <summary>
        ///     Allows kicking members.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        KickMembers			= 0x00_00_00_02,
        /// <summary>
        ///     Allows banning members.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        BanMembers			= 0x00_00_00_04,
        /// <summary>
        ///     Allows all permissions and bypasses channel permission overwrites.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        Administrator       = 0x00_00_00_08,
        /// <summary>
        ///     Allows management and editing of channels.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageChannels      = 0x00_00_00_10,
        /// <summary>
        ///     Allows management and editing of the guild.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageGuild         = 0x00_00_00_20,

        // Text
		/// <summary>
		///     Allows for the addition of reactions to messages.
		/// </summary>
        AddReactions        = 0x00_00_00_40,
		/// <summary>
		///		Allows for viewing of audit logs.
		/// </summary>
        ViewAuditLog        = 0x00_00_00_80,
        [Obsolete("Use ViewChannel instead.")]
        ReadMessages        = ViewChannel,
        ViewChannel         = 0x00_00_04_00,
        SendMessages        = 0x00_00_08_00,
		/// <summary>
		///     Allows for sending of text-to-speech messages.
		/// </summary>
        SendTTSMessages     = 0x00_00_10_00,
        /// <summary>
        ///     Allows for deletion of other users messages.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageMessages      = 0x00_00_20_00,
        /// <summary>
        ///     Allows links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks			= 0x00_00_40_00,
        /// <summary>
        ///     Allows for uploading images and files.
        /// </summary>
        AttachFiles			= 0x00_00_80_00,
        /// <summary>
        ///     Allows for reading of message history.
        /// </summary>
        ReadMessageHistory	= 0x00_01_00_00,
        /// <summary>
        ///     Allows for using the @everyone tag to notify all users in a channel, and the @here tag to notify all
        ///     online users in a channel.
        /// </summary>
        MentionEveryone		= 0x00_02_00_00,
        /// <summary>
        ///     Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis	= 0x00_04_00_00,


        // Voice
        /// <summary>
        ///     Allows for joining of a voice channel.
        /// </summary>
        Connect             = 0x00_10_00_00,
        /// <summary>
        ///     Allows for speaking in a voice channel.
        /// </summary>
        Speak               = 0x00_20_00_00,
        /// <summary>
        ///     Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers         = 0x00_40_00_00,
        /// <summary>
        ///     Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers       = 0x00_80_00_00,
        /// <summary>
        ///     Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers         = 0x01_00_00_00,
        /// <summary>
        ///     Allows for using voice-activity-detection in a voice channel.
        /// </summary>
        UseVAD              = 0x02_00_00_00,
        PrioritySpeaker     = 0x00_00_01_00,

        // General 2
        /// <summary>
        ///     Allows for modification of own nickname.
        /// </summary>
        ChangeNickname		= 0x04_00_00_00,
        /// <summary>
        ///     Allows for modification of other users nicknames.
        /// </summary>
        ManageNicknames     = 0x08_00_00_00,
        /// <summary>
        ///     Allows management and editing of roles.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageRoles         = 0x10_00_00_00,
        /// <summary>
        ///     Allows management and editing of webhooks.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageWebhooks      = 0x20_00_00_00,
        /// <summary>
        ///     Allows management and editing of emojis.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageEmojis        = 0x40_00_00_00
    }
}

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
        CreateInstantInvite = 1L << 0,

        /// <summary>
        ///     Allows kicking members.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        KickMembers = 1L << 1,

        /// <summary>
        ///     Allows banning members.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        BanMembers = 1L << 2,

        /// <summary>
        ///     Allows all permissions and bypasses channel permission overwrites.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        Administrator = 1L << 3,

        /// <summary>
        ///     Allows management and editing of channels.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageChannels = 1L << 4,

        /// <summary>
        ///     Allows management and editing of the guild.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageGuild = 1L << 5,

        /// <summary>
        ///     Allows for viewing of guild insights
        /// </summary>
        ViewGuildInsights = 1L << 19,

        // Text
        /// <summary>
        ///     Allows for the addition of reactions to messages.
        /// </summary>
        AddReactions = 1L << 6,

        /// <summary>
        ///		Allows for viewing of audit logs.
        /// </summary>
        ViewAuditLog = 1L << 7,

        /// <summary>
        ///     Allows guild members to view a channel, which includes reading messages in text channels.
        /// </summary>
        ViewChannel = 1L << 10,

        /// <summary>
        ///     Allows for sending messages in a channel
        /// </summary>
        SendMessages = 1L << 11,

        /// <summary>
        ///     Allows for sending of text-to-speech messages.
        /// </summary>
        SendTTSMessages = 1L << 12,

        /// <summary>
        ///     Allows for deletion of other users messages.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageMessages = 1L << 13,
        /// <summary>
        ///     Allows links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks = 1L << 14,

        /// <summary>
        ///     Allows for uploading images and files.
        /// </summary>
        AttachFiles = 1L << 15,

        /// <summary>
        ///     Allows for reading of message history.
        /// </summary>
        ReadMessageHistory = 1 << 16,

        /// <summary>
        ///     Allows for using the @everyone tag to notify all users in a channel, and the @here tag to notify all
        ///     online users in a channel.
        /// </summary>
        MentionEveryone = 1L << 17,

        /// <summary>
        ///     Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis = 1L << 18,
        
        /// <summary>
        ///     Allows for joining of a voice channel.
        /// </summary>
        Connect = 1L << 20,

        /// <summary>
        ///     Allows for speaking in a voice channel.
        /// </summary>
        Speak = 1L << 21,

        /// <summary>
        ///     Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers = 1L << 22,

        /// <summary>
        ///     Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers = 1L << 23,

        /// <summary>
        ///     Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers = 1L << 24,

        /// <summary>
        ///     Allows for using voice-activity-detection in a voice channel.
        /// </summary>
        UseVAD = 1L << 25,

        /// <summary>
        ///     Allows for using priority speaker in a voice channel.	
        /// </summary>
        PrioritySpeaker = 1L << 8,

        /// <summary>
        ///     Allows video streaming in a voice channel.
        /// </summary>
        Stream = 1L << 9,
        
        // General 2
        /// <summary>
        ///     Allows for modification of own nickname.
        /// </summary>
        ChangeNickname = 1L << 26,

        /// <summary>
        ///     Allows for modification of other users nicknames.
        /// </summary>
        ManageNicknames = 1L << 27,
        /// <summary>
        ///     Allows management and editing of roles.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageRoles = 1L << 28,

        /// <summary>
        ///     Allows management and editing of webhooks.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageWebhooks = 1L << 29,

        /// <summary>
        ///     Allows management and editing of emojis and stickers.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageEmojisAndStickers = 1L << 30,

        /// <summary>
        ///     Allows members to use application commands like slash commands and context menus in text channels.	
        /// </summary>
        UseApplicationCommands = 1L << 31,

        /// <summary>
        ///     Allows for requesting to speak in stage channels.
        /// </summary>
        RequestToSpeak = 1L << 32,

        /// <summary>
        ///     Allows for creating, editing, and deleting guild scheduled events.
        /// </summary>
        ManageEvents = 1L << 33,

        /// <summary>
        ///     Allows for deleting and archiving threads, and viewing all private threads.
        /// </summary>
        /// <remarks>
        ///     This permission requires the owner account to use two-factor
        ///     authentication when used on a guild that has server-wide 2FA enabled.
        /// </remarks>
        ManageThreads = 1L << 34,

        /// <summary>
        ///     Allows for creating public threads.
        /// </summary>
        CreatePublicThreads = 1L << 35,

        /// <summary>
        ///     Allows for creating private threads.
        /// </summary>
        CreatePrivateThreads = 1L << 36,

        /// <summary>
        ///     Allows the usage of custom stickers from other servers.
        /// </summary>
        UseExternalStickers = 1L << 37,

        /// <summary>
        ///     Allows for sending messages in threads.
        /// </summary>
        SendMessagesInThreads = 1L << 38,

        /// <summary>
        ///     Allows for launching activities (applications with the EMBEDDED flag) in a voice channel.
        /// </summary>
        StartEmbeddedActivities = 1L << 39,

        /// <summary>
        ///     Allows for timing out users.
        /// </summary>
        ModerateMembers = 1L << 40,

        /// <summary>
        ///     Allows for viewing role subscription insights.
        /// </summary>
        ViewMonetizationAnalytics = 1L << 41,

        /// <summary>
        ///     Allows for using the soundboard.
        /// </summary>
        UseSoundboard = 1L << 42,

        /// <summary>
        ///     Allows for creating emojis, stickers, and soundboard sounds, and editing and deleting those created by the current user.
        /// </summary>
        CreateGuildExpressions = 1L << 43,

        /// <summary>
        ///     Allows sending voice messages.
        /// </summary>
        SendVoiceMessages = 1L << 46,

        /// <summary>
        ///     Allows members to interact with the Clyde AI bot.
        /// </summary>
        UseClydeAI = 1L << 47,
        
        /// <summary>
        ///     Allows setting voice channel status.
        /// </summary>
        SetVoiceChannelStatus = 1L << 48,

        /// <summary>
        ///     Allows sending polls.
        /// </summary>
        SendPolls = 1L << 49,
    }
}

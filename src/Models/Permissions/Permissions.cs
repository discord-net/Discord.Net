using System;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents the permissions flags.
    /// </summary>
    [Flags]
    public enum Permissions : ulong
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allows creation of instant invites.
        /// </summary>
        CreateInstantInvite = 1 << 0,

        /// <summary>
        /// Allows kicking members.
        /// </summary>
        KickMembers = 1 << 1,

        /// <summary>
        /// Allows banning members.
        /// </summary>
        BanMembers = 1 << 2,

        /// <summary>
        /// Allows all permissions and bypasses channel permission overwrites.
        /// </summary>
        Administrator = 1 << 3,

        /// <summary>
        /// Allows management and editing of channels.
        /// </summary>
        ManageChannels = 1 << 4,

        /// <summary>
        /// Allows management and editing of the guild.
        /// </summary>
        ManageGuild = 1 << 5,

        /// <summary>
        /// Allows for the addition of reactions to messages.
        /// </summary>
        AddReactions = 1 << 6,

        /// <summary>
        /// Allows for viewing of audit logs.
        /// </summary>
        ViewAuditLog = 1 << 7,

        /// <summary>
        /// Allows for using priority speaker in a voice channel.
        /// </summary>
        PrioritySpeaker = 1 << 8,

        /// <summary>
        /// Allows the user to go live.
        /// </summary>
        Stream = 1 << 9,

        /// <summary>
        /// Allows guild members to view a channel, which includes reading messages in text channels.
        /// </summary>
        ViewChannel = 1 << 10,

        /// <summary>
        /// Allows for sending messages in a channel.
        /// </summary>
        SendMessages = 1 << 11,

        /// <summary>
        /// Allows for sending of /tts messages.
        /// </summary>
        SendTtsMessages = 1 << 12,

        /// <summary>
        /// Allows for deletion of other users messages.
        /// </summary>
        ManageMessages = 1 << 13,

        /// <summary>
        /// Links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks = 1 << 14,

        /// <summary>
        /// Allows for uploading images and files.
        /// </summary>
        AttachFiles = 1 << 15,

        /// <summary>
        /// Allows for reading of message history.
        /// </summary>
        ReadMessageHistory = 1 << 16,

        /// <summary>
        /// Allows for using the @everyone tag to notify all users in a channel, and the @here tag to notify all online users in a channel.
        /// </summary>
        MentionEveryone = 1 << 17,

        /// <summary>
        /// Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis = 1 << 18,

        /// <summary>
        /// Allows for viewing guild insights.
        /// </summary>
        ViewGuildInsights = 1 << 19,

        /// <summary>
        /// Allows for joining of a voice channel.
        /// </summary>
        Connect = 1 << 20,

        /// <summary>
        /// Allows for speaking in a voice channel.
        /// </summary>
        Speak = 1 << 21,

        /// <summary>
        /// Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers = 1 << 22,

        /// <summary>
        /// Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers = 1 << 23,

        /// <summary>
        /// Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers = 1 << 24,

        /// <summary>
        /// Allows for using voice-activity-detection in a voice channel.
        /// </summary>
        UseVad = 1 << 25,

        /// <summary>
        /// Allows for modification of own nickname.
        /// </summary>
        ChangeNickname = 1 << 26,

        /// <summary>
        /// Allows for modification of other users nicknames.
        /// </summary>
        ManageNicknames = 1 << 27,

        /// <summary>
        /// Allows management and editing of roles.
        /// </summary>
        ManageRoles = 1 << 28,

        /// <summary>
        /// Allows management and editing of webhooks.
        /// </summary>
        ManageWebhooks = 1 << 29,

        /// <summary>
        /// Allows management and editing of emojis.
        /// </summary>
        ManageEmojis = 1 << 30,

        /// <summary>
        /// Allows members to use slash commands in text channels.
        /// </summary>
        UseSlashCommands = 1UL << 31,

        /// <summary>
        /// Allows for requesting to speak in stage channels. (This permission is under active development and may be changed or removed.).
        /// </summary>
        RequestToSpeak = 1UL << 32,

        /// <summary>
        /// Allows for deleting and archiving threads, and viewing all private threads.
        /// </summary>
        ManageThreads = 1UL << 34,

        /// <summary>
        /// Allows for creating and participating in threads.
        /// </summary>
        UsePublicThreads = 1UL << 35,

        /// <summary>
        /// Allows for creating and participating in private threads.
        /// </summary>
        UsePrivateThreads = 1UL << 36,
    }
}

using System;

namespace Discord
{
    /// <summary> Defines the available permissions for a channel. </summary>
    [Flags]
    public enum ChannelPermission : ulong
    {
        // General
        /// <summary>
        ///     Allows creation of instant invites.
        /// </summary>
        CreateInstantInvite	= 0x00_00_00_01,
        /// <summary>
        ///     Allows management and editing of channels.
        /// </summary>
        ManageChannels		= 0x00_00_00_10,

        // Text
        /// <summary>
        ///     Allows for the addition of reactions to messages.
        /// </summary>
        AddReactions		= 0x00_00_00_40,
        /// <summary>
        ///     Allows for reading of messages. This flag is obsolete, use <see cref = "ViewChannel" /> instead.
        /// </summary>
        [Obsolete("Use ViewChannel instead.")]
        ReadMessages		= ViewChannel,
        /// <summary>
        ///     Allows guild members to view a channel, which includes reading messages in text channels.
        /// </summary>
        ViewChannel			= 0x00_00_04_00,
        /// <summary>
        ///     Allows for sending messages in a channel.
        /// </summary>
        SendMessages		= 0x00_00_08_00,
        /// <summary>
        ///     Allows for sending of text-to-speech messages.
        /// </summary>
        SendTTSMessages		= 0x00_00_10_00,
        /// <summary>
        ///     Allows for deletion of other users messages.
        /// </summary>
        ManageMessages		= 0x00_00_20_00,
        /// <summary>
        ///     Allows links sent by users with this permission will be auto-embedded.
        /// </summary>
        EmbedLinks			= 0x00_00_40_00,
        /// <summary>
        ///     Allows for uploading images and files.
        /// </summary>
        AttachFiles         = 0x00_00_80_00,
        /// <summary>
        ///     Allows for reading of message history.
        /// </summary>
        ReadMessageHistory	= 0x00_01_00_00,
        /// <summary>
        ///     Allows for using the @everyone tag to notify all users in a channel, and the @here tag to notify all
        ///     online users in a channel.
        /// </summary>
        MentionEveryone     = 0x00_02_00_00,
        /// <summary>
        ///     Allows the usage of custom emojis from other servers.
        /// </summary>
        UseExternalEmojis   = 0x00_04_00_00,

        // Voice
        /// <summary>
        ///     Allows for joining of a voice channel.
        /// </summary>
        Connect				= 0x00_10_00_00,
        /// <summary>
        ///     Allows for speaking in a voice channel.
        /// </summary>
        Speak				= 0x00_20_00_00,
        /// <summary>
        ///     Allows for muting members in a voice channel.
        /// </summary>
        MuteMembers			= 0x00_40_00_00,
        /// <summary>
        ///     Allows for deafening of members in a voice channel.
        /// </summary>
        DeafenMembers		= 0x00_80_00_00,
        /// <summary>
        ///     Allows for moving of members between voice channels.
        /// </summary>
        MoveMembers         = 0x01_00_00_00,
        /// <summary>
        ///     Allows for using voice-activity-detection in a voice channel.
        /// </summary>
        UseVAD         		= 0x02_00_00_00,
        PrioritySpeaker     = 0x00_00_01_00,

        // More General
        /// <summary>
        ///     Allows management and editing of roles.
        /// </summary>
        ManageRoles         = 0x10_00_00_00,
        /// <summary>
        ///     Allows management and editing of webhooks.
        /// </summary>
        ManageWebhooks		= 0x20_00_00_00,
    }
}

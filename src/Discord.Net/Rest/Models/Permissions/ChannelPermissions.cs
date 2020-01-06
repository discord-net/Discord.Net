using System;

namespace Discord.Models
{
    [Flags]
    public enum ChannelPermissions : ulong
    {
        // General
        CreateInstantInvite = 0x0000_0001,
        ManageChannel = 0x0000_0010,
        AddReactions = 0x0000_0040,
        ViewChannel = 0x0000_0400,
        ManagePermissions = 0x1000_0000,
        ManageWebhooks = 0x2000_0000,

        // Messages
        SendMessages = 0x0000_0800,
        SendTtsMessages = 0x0000_0100,
        ManageMessages = 0x0000_02000,
        EmbedLinks = 0x0000_4000,
        AttachFiles = 0x0000_8000,
        ReadMessageHistory = 0x0001_0000,
        MentionEveryone = 0x0002_0000,
        UseExternalEmoji = 0x0004_0000,

        // Voice
        Connect = 0x0010_0000,
        Speak = 0x0020_0000,
        MuteMembers = 0x0040_0000,
        DeafenMembers = 0x0080_0000,
        MoveMembers = 0x0100_0000,
        UseVoiceActivity = 0x0200_0000,
        PrioritySpeaker = 0x0000_0100,
        Stream = 0x0000_0200,
    }
}

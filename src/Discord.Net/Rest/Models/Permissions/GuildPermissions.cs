using System;

namespace Discord.Models
{
    // todo: doc these when other models exist
    [Flags]
    public enum GuildPermissions : ulong
    {
        // General
        CreateInstantInvite = 0x0000_0001,
        KickMembers = 0x0000_0002,
        BanMembers = 0x0000_0004,
        Administrator = 0x0000_0008,
        ManageChannels = 0x0000_0010,
        ManageGuild = 0x0000_0020,
        AddReactions = 0x0000_0040,
        ViewAuditLog = 0x0000_0080,
        ViewChannel = 0x0000_0400,
        ChangeNickname = 0x0400_0000,
        ManageNicknames = 0x0800_0000,
        ManageRoles = 0x1000_0000,
        ManageWebhooks = 0x2000_0000,
        ManageEmoji = 0x4000_0000,

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

using System;
using Discord.Net.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a guild or DM channel within Discord.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-object-channel-structure"/>
    /// </remarks>
    /// <param name="Id">
    /// The id of this channel.
    /// </param>
    /// <param name="Type">
    /// The type of channel.
    /// </param>
    [DiscriminatedUnion(nameof(Channel.Type))]
    [GenerateSerializer]
    public record Channel(
        ChannelType Type,
        Snowflake Id);

    /// <summary>
    /// Represents a text channel within a server.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildText)]
    [GenerateSerializer]
    public record GuildTextChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        /*Overwrite[] PermissionOverwrites,*/
        string Name,
        string? Topic,
        bool Nsfw,
        Snowflake LastMessageId,
        int RateLimitPerUser,
        Snowflake? ParentId,
        DateTimeOffset? LastPinTimestamp)
        : Channel(
            ChannelType.GuildText,
            Id);

    /*

    /// <summary>
    /// Represents a direct message between users.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.DM)]
    [GenerateSerializer]
    public record DMChannel(
        Snowflake Id,
        User[] Recipients)
        : Channel(
            ChannelType.DM,
            Id);

    /// <summary>
    /// Represents a voice channel within a server.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildVoice)]
    [GenerateSerializer]
    public record GuildVoiceChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites,
        string Name,
        bool Nsfw,
        int Bitrate,
        int UserLimit,
        Snowflake? ParentId,
        string? RtcRegion,
        int VideoQualityMode)
        : Channel(
            ChannelType.GuildVoice,
            Id);

    /// <summary>
    /// Represents a direct message between multiple users.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GroupDM)]
    [GenerateSerializer]
    public record GroupDMChannel(
        Snowflake Id,
        string Name,
        Snowflake LastMessageId,
        User[] Recipients,
        string? Icon,
        Snowflake? OwnerId,
        Snowflake? ApplicationId,
        DateTimeOffset? LastPinTimestamp)
        : Channel(
            ChannelType.GroupDM,
            Id);

    /// <summary>
    /// Represents an organizational category that contains up to 50 channels.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildCategory)]
    [GenerateSerializer]
    public record GuildCategoryChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites,
        string Name)
        : Channel(
            ChannelType.GuildCategory,
            Id);

    /// <summary>
    /// Represents a channel that users can follow and crosspost into their own
    /// server.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildNews)]
    [GenerateSerializer]
    public record GuildNewsChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites,
        string Name,
        string? Topic,
        bool Nsfw,
        Snowflake? LastMessageId,
        int RateLimitPerUser,
        Snowflake? ParentId,
        Snowflake? LastPinTimestamp)
        : Channel(
            ChannelType.GuildNews,
            Id);

    /// <summary>
    /// Represents a channel in which game developers can sell their game on
    /// Discord.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildStore)]
    [GenerateSerializer]
    public record GuildStoreChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites, // I guess???
        string? Name,
        Snowflake? ParentId)
        : Channel(
            ChannelType.GuildStore,
            Id);

    /// <summary>
    /// Represents a temporary sub-channel within a
    /// <see cref="GuildNewsChannel"/>.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildNewsThread)]
    [GenerateSerializer]
    public record GuildNewsThreadChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites, // I guess??
        string Name,
        Snowflake? LastMessageId,
        Snowflake? ParentId,
        Snowflake? LastPinTimestamp,
        int MessageCount,
        int MemberCount,
        ThreadMetadata ThreadMetadata,
        ThreadMember Member)
        : Channel(
            ChannelType.GuildNewsThread,
            Id);

    /// <summary>
    /// Represents a temporary sub-channel within a
    /// <see cref="GuildTextChannel"/>.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildPublicThread)]
    [GenerateSerializer]
    public record GuildPublicThreadChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites, // I guess??
        string Name,
        Snowflake? LastMessageId,
        Snowflake? ParentId,
        Snowflake? LastPinTimestamp,
        int MessageCount,
        int MemberCount,
        ThreadMetadata ThreadMetadata,
        ThreadMember Member)
        : Channel(
            ChannelType.GuildPublicThread,
            Id);

    /// <summary>
    /// Represents a temporary sub-channel within a
    /// <see cref="GuildTextChannel"/> that is only viewable by those invited
    /// and those with the MANAGE_THREADS permission.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildPrivateThread)]
    [GenerateSerializer]
    public record GuildPrivateThreadChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites, // I guess???
        string Name,
        Snowflake? LastMessageId,
        Snowflake? ParentId,
        Snowflake? LastPinTimestamp,
        int MessageCount,
        int MemberCount,
        ThreadMetadata ThreadMetadata,
        ThreadMember Member)
        : Channel(
            ChannelType.GuildPrivateThread,
            Id);

    /// <summary>
    /// Represents a voice channel for hosting events with an audience.
    /// </summary>
    [DiscriminatedUnionMember(ChannelType.GuildStageVoice)]
    [GenerateSerializer]
    public record GuildStageVoiceChannel(
        Snowflake Id,
        Snowflake GuildId,
        int Position,
        Overwrite[] PermissionOverwrites,
        string Name,
        int Bitrate,
        int UserLimit,
        string? RtcRegion)
        : Channel(
            ChannelType.GuildStageVoice,
            Id);

    */
}

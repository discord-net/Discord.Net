namespace Discord;

/// <summary> Defines the types of channels. </summary>
public enum ChannelType
{
    /// <summary> The channel is a text channel. </summary>
    [TypeHeuristic<ITextChannel>]
    Text = 0,

    /// <summary> The channel is a Direct Message channel. </summary>
    [TypeHeuristic<IDMChannel>]
    DM = 1,

    /// <summary> The channel is a voice channel. </summary>
    [TypeHeuristic<IVoiceChannel>]
    Voice = 2,

    /// <summary> The channel is a group channel. </summary>
    [TypeHeuristic<IGroupChannel>]
    Group = 3,

    /// <summary> The channel is a category channel. </summary>
    [TypeHeuristic<ICategoryChannel>]
    Category = 4,

    /// <summary> The channel is a news channel. </summary>
    [TypeHeuristic<INewsChannel>]
    News = 5,

    /// <summary> The channel is a store channel. </summary>
    Store = 6,

    /// <summary> The channel is a temporary thread channel under a news channel. </summary>
    [TypeHeuristic<IThreadChannel>]
    NewsThread = 10,

    /// <summary> The channel is a temporary thread channel under a text channel.  </summary>
    [TypeHeuristic<IThreadChannel>]
    PublicThread = 11,

    /// <summary> The channel is a private temporary thread channel under a text channel.  </summary>
    [TypeHeuristic<IThreadChannel>]
    PrivateThread = 12,

    /// <summary> The channel is a stage voice channel. </summary>
    [TypeHeuristic<IStageChannel>]
    Stage = 13,

    /// <summary> The channel is a guild directory used in hub servers. (Unreleased)</summary>
    GuildDirectory = 14,

    /// <summary> The channel is a forum channel containing multiple threads. </summary>
    [TypeHeuristic<IForumChannel>]
    Forum = 15,

    /// <summary> The channel is a media channel.</summary>
    [TypeHeuristic<IMediaChannel>]
    Media = 16
}

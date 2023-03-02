using System;

namespace Discord;

/// <summary>
///     
/// </summary>
public interface IGuildOnboardingPromptOption : ISnowflakeEntity
{
    /// <summary>
    /// 
    /// </summary>
    ulong[] ChannelIds { get; }

    /// <summary>
    /// 
    /// </summary>
    ulong[] RoleIds { get; }

    /// <summary>
    /// 
    /// </summary>
    IRole[] Roles { get; }

    /// <summary>
    /// 
    /// </summary>
    IEmote Emoji { get; }

    /// <summary>
    /// 
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 
    /// </summary>
    string Description { get; }
}

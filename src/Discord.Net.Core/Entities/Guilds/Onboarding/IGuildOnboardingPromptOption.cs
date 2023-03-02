using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public interface IGuildOnboardingPromptOption : ISnowflakeEntity
{
    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<ulong> ChannelIds { get; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<ulong> RoleIds { get; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<IRole> Roles { get; }

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

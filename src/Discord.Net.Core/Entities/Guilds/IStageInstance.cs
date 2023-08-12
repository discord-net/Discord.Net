using System;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents a stage instance.
/// </summary>
public interface IStageInstance : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the guild id of the associated Stage channel.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    ///     Gets the id of the associated Stage channel.
    /// </summary>
    public ulong ChannelId { get; }

    /// <summary>
    ///     Gets the topic of the Stage instance.
    /// </summary>
    public string Topic { get; }

    /// <summary>
    ///     Gets the privacy level of the Stage instance.
    /// </summary>
    public StagePrivacyLevel PrivacyLevel { get; }
    
    /// <summary>
    ///     Gets the id of the scheduled event for this Stage instance.
    /// </summary>
    public ulong? ScheduledEventId { get; }

    /// <summary>
    ///     Modifies the stage instance.
    /// </summary>
    Task ModifyAsync(Action<StageInstanceProperties> func, RequestOptions options = null);

    /// <summary>
    ///     Stops the stage instance.
    /// </summary>
    Task StopAsync(RequestOptions options = null);
}

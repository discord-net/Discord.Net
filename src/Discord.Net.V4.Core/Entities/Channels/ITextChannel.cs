using Discord.Entities.Channels.Threads;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable = IModifiable<ulong, ITextChannel, ModifyTextChannelProperties, ModifyGuildChannelParams>;

/// <summary>
///     Represents a generic channel in a guild that can send and receive messages.
/// </summary>
public interface ITextChannel :
    IMessageChannel,
    IMentionable,
    INestedChannel,
    IIntegrationChannel,
    Modifiable
{
    /// <summary>
    ///     Gets a value that indicates whether the channel is NSFW.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the channel has the NSFW flag enabled; otherwise <see langword="false" />.
    /// </returns>
    bool IsNsfw { get; }

    /// <summary>
    ///     Gets the current topic for this text channel.
    /// </summary>
    /// <returns>
    ///     A string representing the topic set in the channel; <see langword="null" /> if none is set.
    /// </returns>
    string? Topic { get; }

    /// <summary>
    ///     Gets the current slow-mode delay for this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    /// </returns>
    int SlowModeInterval { get; }

    /// <summary>
    ///     Gets the current default slow-mode delay for threads in this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    /// </returns>
    int DefaultSlowModeInterval { get; }

    /// <summary>
    ///     Gets the default auto-archive duration for client-created threads in this channel.
    /// </summary>
    /// <remarks>
    ///     The value of this property does not affect API thread creation, it will not respect this value.
    /// </remarks>
    /// <returns>
    ///     The default auto-archive duration for thread creation in this channel.
    /// </returns>
    ThreadArchiveDuration DefaultArchiveDuration { get; }

    static ApiBodyRoute<ModifyGuildChannelParams> Modifiable.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}

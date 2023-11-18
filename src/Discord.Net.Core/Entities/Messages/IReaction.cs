using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a generic reaction object.
/// </summary>
public interface IReaction
{
    /// <summary>
    ///     The <see cref="IEmote" /> used in the reaction.
    /// </summary>
    IEmote Emote { get; }

    /// <summary>
    ///     Gets colors used for the super reaction.
    /// </summary>
    /// <remarks>
    ///     The collection will be empty if the reaction is a normal reaction.
    /// </remarks>
    public IReadOnlyCollection<Color> BurstColors { get; }
}

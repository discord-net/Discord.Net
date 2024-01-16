using System.Collections.Generic;

namespace Discord;

/// <summary>
///     A metadata containing reaction information.
/// </summary>
public struct ReactionMetadata
{
    /// <summary>
    ///     Gets the number of reactions.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> representing the number of this reactions that has been added to this message.
    /// </returns>
    public int ReactionCount { get; internal set; }
  
    /// <summary>
    ///     Gets a value that indicates whether the current user has reacted to this.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user has reacted to the message; otherwise <see langword="false" />.
    /// </returns>
    public bool IsMe { get; internal set; }

    /// <summary>
    ///     Gets the number of burst reactions added to this message.
    /// </summary>
    public int BurstCount { get; internal set; }

    /// <summary>
    ///     Gets the number of normal reactions added to this message.
    /// </summary>
    public int NormalCount { get; internal set; }

    /// <summary>
    ///     Gets colors used for super reaction.
    /// </summary>
    public IReadOnlyCollection<Color> BurstColors { get; internal set; }
}

using Discord.Models;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     A metadata containing reaction information.
/// </summary>
public readonly struct ReactionMetadata(
    int totalReactionCount,
    bool isMe,
    int normalReactionCount,
    int burstReactionCount,
    bool isMeBurst,
    IReadOnlyCollection<string> burstColors
) : IModelConstructable<ReactionMetadata, IReactionModel>, IEquatable<ReactionMetadata>
{
    /// <summary>
    ///     The number of reactions.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the number of this reactions that has been added to this message.
    /// </returns>
    public readonly int TotalReactionCount = totalReactionCount;

    public readonly int NormalReactionCount = normalReactionCount;
    public readonly int BurstReactionCount = burstReactionCount;

    /// <summary>
    ///     A value that indicates whether the current user has reacted to this.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user has reacted to the message; otherwise <see langword="false" />.
    /// </returns>
    public readonly bool IsMe = isMe;

    public readonly bool IsMeBurst = isMeBurst;
    public readonly IReadOnlyCollection<string> BurstColors = burstColors;

    public static ReactionMetadata Construct(IDiscordClient client, IReactionModel model)
        => new(
            model.Total,
            model.Me,
            model.NormalCount,
            model.BurstCount,
            model.MeBurst,
            model.BurstColors.ToImmutableArray()
        );

    public bool Equals(ReactionMetadata other)
        => TotalReactionCount == other.TotalReactionCount &&
           NormalReactionCount == other.NormalReactionCount &&
           BurstReactionCount == other.BurstReactionCount &&
           IsMe == other.IsMe &&
           IsMeBurst == other.IsMeBurst &&
           BurstColors.SequenceEqual(other.BurstColors);

    public override bool Equals(object? obj) => obj is ReactionMetadata other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(TotalReactionCount, NormalReactionCount, BurstReactionCount, IsMe, IsMeBurst, BurstColors);

    public static bool operator ==(ReactionMetadata left, ReactionMetadata right) => left.Equals(right);

    public static bool operator !=(ReactionMetadata left, ReactionMetadata right) => !left.Equals(right);
}

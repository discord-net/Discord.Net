using System.Diagnostics;

namespace Discord;

/// <summary>
///     An activity object found in a sent message.
/// </summary>
/// <remarks>
///     <para>
///         This class refers to an activity object, visually similar to an embed within a message. However, a message
///         activity is interactive as opposed to a standard static embed.
///     </para>
///     <para>For example, a Spotify party invitation counts as a message activity.</para>
/// </remarks>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct MessageActivity
{
    /// <summary>
    ///     The type of activity of this message.
    /// </summary>
    public readonly MessageActivityType Type;
    /// <summary>
    ///     The party ID of this activity.
    /// </summary>
    public readonly string? PartyId;

    internal MessageActivity(MessageActivityType type, string? partyId = null)
    {
        Type = type;
        PartyId = partyId;
    }

    private string DebuggerDisplay
        => $"{Type}{(string.IsNullOrWhiteSpace(PartyId) ? "" : $" {PartyId}")}";

    public override string ToString() => DebuggerDisplay;
}

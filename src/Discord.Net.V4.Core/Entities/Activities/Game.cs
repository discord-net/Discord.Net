using System.Diagnostics;

namespace Discord;

/// <summary>
///     A user's game status.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class Game : IActivity
{
    /// <inheritdoc/>
    public string Name { get; init; }

    /// <inheritdoc/>
    public ActivityType Type { get; init; }

    /// <inheritdoc/>
    public ActivityProperties Flags { get; init; }

    /// <inheritdoc/>
    public string? Details { get; init; }

    internal Game() { }

    /// <summary>
    ///     Creates a <see cref="Game"/> with the provided name and <see cref="ActivityType"/>.
    /// </summary>
    /// <param name="name">The name of the game.</param>
    /// <param name="type">The type of activity.</param>
    public Game(string name, ActivityType type = ActivityType.Playing, ActivityProperties flags = ActivityProperties.None, string? details = null)
    {
        Name = name;
        Type = type;
        Flags = flags;
        Details = details;
    }

    /// <summary> Returns the name of the <see cref="Game"/>. </summary>
    public override string ToString() => Name;
    private string DebuggerDisplay => Name;
}

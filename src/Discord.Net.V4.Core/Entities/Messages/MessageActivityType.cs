namespace Discord;

/// <summary>
///     Represents the different types of activities.
/// </summary>
public enum MessageActivityType
{
    /// <summary>
    ///     A joinable activity.
    /// </summary>
    Join = 1,

    /// <summary>
    ///     A spectateable activity.
    /// </summary>
    Spectate = 2,

    /// <summary>
    ///     A listenable activity.
    /// </summary>
    Listen = 3,

    /// <summary>
    ///     An activity that can be requested to join.
    /// </summary>
    JoinRequest = 5
}

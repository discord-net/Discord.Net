namespace Discord;

/// <summary>
///     Represents an object for storing a CustomId wild card match.
/// </summary>
internal record RouteSegmentMatch(string Value) : IRouteSegmentMatch
{
    /// <inheritdoc/>
    public string Value { get; } = Value;
}

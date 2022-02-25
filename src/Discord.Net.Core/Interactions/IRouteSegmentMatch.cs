namespace Discord
{
    /// <summary>
    ///     Represents an object for storing CustomId a wild card match.
    /// </summary>
    public interface IRouteSegmentMatch
    {
        /// <summary>
        ///     Gets the captured value.
        /// </summary>
        string Value { get; }
    }
}

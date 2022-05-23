namespace Discord
{
    /// <summary>
    ///     Represents an object for storing a CustomId wild card match.
    /// </summary>
    public interface IRouteSegmentMatch
    {
        /// <summary>
        ///     Gets the captured value of this wild card match.
        /// </summary>
        /// <returns>
        ///    The value of this wild card.
        /// </returns>
        string Value { get; }
    }
}

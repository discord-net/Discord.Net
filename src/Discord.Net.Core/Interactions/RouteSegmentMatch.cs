namespace Discord
{
    /// <summary>
    ///     Represents an object for storing CustomId wild card matches.
    /// </summary>
    public record RouteSegmentMatch : IRouteSegmentMatch
    {
        /// <inheritdoc/>
        public string Value { get; }

        internal RouteSegmentMatch(string value)
        {
            Value = value;
        }
    }
}

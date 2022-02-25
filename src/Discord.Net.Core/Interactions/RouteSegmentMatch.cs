namespace Discord
{
    /// <summary>
    ///     Represents an object for storing CustomId wild card matches.
    /// </summary>
    internal record RouteSegmentMatch : IRouteSegmentMatch
    {
        /// <inheritdoc/>
        public string Value { get; }

        public RouteSegmentMatch(string value)
        {
            Value = value;
        }
    }
}

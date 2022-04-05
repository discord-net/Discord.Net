namespace Discord
{
    /// <summary>
    ///     Represents an object for storing a CustomId wild card match.
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

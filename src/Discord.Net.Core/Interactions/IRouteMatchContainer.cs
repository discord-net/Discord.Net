using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a container for temporarily storing CustomId wild card matches of a command name.
    /// </summary>
    public interface IRouteMatchContainer
    {
        /// <summary>
        ///     Gets the collection of the captured route segments.
        /// </summary>
        IEnumerable<IRouteSegmentMatch> SegmentMatches { get; }

        /// <summary>
        ///     Sets the <see cref="SegmentMatches"/> propert
        /// </summary>
        /// <param name="segmentMatches">The collection of captured route segments.</param>
        void SetSegmentMatches(IEnumerable<IRouteSegmentMatch> segmentMatches);
    }
}

using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a container for temporarily storing CustomId wild card matches of a component.
    /// </summary>
    public interface IRouteMatchContainer
    {
        /// <summary>
        ///     Gets the collection of captured route segments in this container.
        /// </summary>
        /// <returns>
        ///    A collection of captured route segments.
        ///</returns>
        IEnumerable<IRouteSegmentMatch> SegmentMatches { get; }

        /// <summary>
        ///     Sets the <see cref="SegmentMatches"/> property of this container.
        /// </summary>
        /// <param name="segmentMatches">The collection of captured route segments.</param>
        void SetSegmentMatches(IEnumerable<IRouteSegmentMatch> segmentMatches);
    }
}

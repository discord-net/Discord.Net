using System;

namespace Discord
{
    /// <summary>
    ///     Provides a series of helper methods for handling sorting parameters.
    /// </summary>
    public static class SortUtils
    {
        /// <summary>
        ///     Used to get the valid string identifier used by discord's sorting direction options.
        /// </summary>
        /// <param name="value">The sorting <see cref="SortDirection"/></param>
        /// <returns>
        ///     A <see cref="string" /> representing the correct string identifier.
        /// </returns>
        public static string ToStringIdentifier(SortDirection value)
            => value switch
            {
                SortDirection.Ascending => "asc",
                SortDirection.Descending => "desc",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown SortDirection value.")
            };

        /// <summary>
        ///     Used to get the valid string identifier used by discord's sorting algorithm options.
        /// </summary>
        /// <param name="value">The sorting <see cref="SortAlgorithm"/></param>
        /// <returns>
        ///     A <see cref="string" /> representing the correct string identifier.
        /// </returns>
        public static string ToStringIdentifier(SortAlgorithm value)
            => value switch
            {
                SortAlgorithm.Timestamp => "timestamp",
                SortAlgorithm.Relevance => "relevance",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown SortAlgorithm value.")
            };
    }
}

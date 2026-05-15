namespace Discord
{
    /// <summary>
    ///     Specifies the sort algorithm of entities (e.g. messages) should be retrieved in.
    /// </summary>
    /// <remarks>
    ///     This enum is used to specify the sort algorithm for retrieving entities.
    /// </remarks>
    public enum SortAlgorithm
    {
        /// <summary>
        ///     The entities should be sorted based on their creation time.
        /// </summary>
        Timestamp,
        /// <summary>
        ///     The entities should be sorted based on their relevance to the search query.
        /// </summary>
        Relevance
    }
}

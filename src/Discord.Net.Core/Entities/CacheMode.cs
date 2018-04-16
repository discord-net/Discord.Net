namespace Discord
{
    /// <summary>
    ///     Specifies the cache mode that should be used.
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        ///     Allows the object to be downloaded if it does not exist in the current cache.
        /// </summary>
        AllowDownload,
        /// <summary>
        ///     Only allows the object to be pulled from the existing cache.
        /// </summary>
        CacheOnly
    }
}

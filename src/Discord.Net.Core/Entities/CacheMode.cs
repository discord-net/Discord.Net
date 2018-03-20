namespace Discord
{
    public enum CacheMode
    {
        /// <summary> Allows the object to be downloaded if it does not exist in the current cache. </summary>
        AllowDownload,
        /// <summary> Only allows the object to be pulled from the existing cache. </summary>
        CacheOnly
    }
}

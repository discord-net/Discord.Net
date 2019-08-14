namespace Discord
{
    /// <summary>
    ///     Specifies the level of precision to request in the rate limit
    ///     response header.
    /// </summary>
    public enum RateLimitPrecision
    {
        /// <summary>
        ///     Specifies precision rounded up to the nearest whole second
        /// </summary>
        Second,
        /// <summary>
        ///     Specifies precision rounded to the nearest millisecond.
        /// </summary>
        Millisecond
    }
}

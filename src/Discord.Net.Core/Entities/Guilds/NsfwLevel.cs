namespace Discord
{
    public enum NsfwLevel
    {
        /// <summary>
        ///     Default or unset.
        /// </summary>
        Default = 0,
        /// <summary>
        ///     Guild has extremely suggestive or mature content that would only be suitable for users 18 or over.
        /// </summary>
        Explicit = 1,
        /// <summary>
        ///     Guild has no content that could be deemed NSFW; in other words, SFW.
        /// </summary>
        Safe = 2,
        /// <summary>
        ///     Guild has mildly NSFW content that may not be suitable for users under 18.
        /// </summary>
        AgeRestricted = 3
    }
}

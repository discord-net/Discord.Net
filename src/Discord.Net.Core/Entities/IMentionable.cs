namespace Discord
{
    /// <summary>
    ///     Determines whether the object is mentionable or not.
    /// </summary>
    public interface IMentionable
    {
        /// <summary>
        ///     Returns a special string used to mention this object.
        /// </summary>
        /// <returns>
        ///     A string that is recognized by Discord as a mention (e.g. &lt;@168693960628371456&gt;).
        /// </returns>
        string Mention { get; }
    }
}

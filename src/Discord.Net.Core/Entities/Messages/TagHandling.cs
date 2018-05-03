namespace Discord
{
    /// <summary>
    ///     Specifies the handling type the tag should use.
    /// </summary>
    /// <seealso cref="MentionUtils"/>
    /// <seealso cref="IUserMessage.Resolve"/>
    public enum TagHandling
    {
        /// <summary> 
        ///     Tag handling is ignored (e.g. &lt;@53905483156684800&gt; -&gt; &lt;@53905483156684800&gt;).
        /// </summary>
        Ignore = 0,
        /// <summary> 
        ///     Removes the tag entirely. 
        /// </summary>
        Remove,
        /// <summary> 
        ///     Resolves to username (e.g. &lt;@53905483156684800&gt; -&gt; @Voltana). 
        /// </summary>
        Name,
        /// <summary> 
        ///     Resolves to username without mention prefix (e.g. &lt;@53905483156684800&gt; -&gt; Voltana). 
        /// </summary>
        NameNoPrefix,
        /// <summary> 
        ///     Resolves to username with discriminator value. (e.g. &lt;@53905483156684800&gt; -&gt; @Voltana#8252). 
        /// </summary>
        FullName,
        /// <summary> 
        ///     Resolves to username with discriminator value without mention prefix. (e.g. &lt;@53905483156684800&gt; -&gt; Voltana#8252). 
        /// </summary>
        FullNameNoPrefix,
        /// <summary> 
        ///     Sanitizes the tag (e.g. &lt;@53905483156684800&gt; -&gt; &lt;@53905483156684800&gt; (w/ nbsp)).
        /// </summary>
        Sanitize
    }
}

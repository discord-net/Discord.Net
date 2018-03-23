namespace Discord
{
    /// <summary> Specifies the handling type the tag should use. </summary>
    public enum TagHandling
    {
        /// <summary> Tag handling is ignored. </summary>
        Ignore = 0,         //<@53905483156684800> -> <@53905483156684800>
        /// <summary> Removes the tag entirely. </summary>
        Remove,             //<@53905483156684800> ->
        /// <summary> Resolves to username (e.g. @User). </summary>
        Name,               //<@53905483156684800> -> @Voltana
        /// <summary> Resolves to username without mention prefix (e.g. User). </summary>
        NameNoPrefix,       //<@53905483156684800> -> Voltana
        /// <summary> Resolves to username with discriminator value. (e.g. @User#0001). </summary>
        FullName,           //<@53905483156684800> -> @Voltana#8252
        /// <summary> Resolves to username with discriminator value without mention prefix. (e.g. User#0001). </summary>
        FullNameNoPrefix,   //<@53905483156684800> -> Voltana#8252
        /// <summary> Sanitizes the tag. </summary>
        Sanitize            //<@53905483156684800> -> <@53905483156684800> (w/ nbsp)
    }
}

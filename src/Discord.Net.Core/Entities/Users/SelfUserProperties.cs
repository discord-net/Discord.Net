namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify the <see cref="ISelfUser" /> with the specified changes.
    /// </summary>
    /// <seealso cref="ISelfUser.ModifyAsync" />
    public class SelfUserProperties
    {
        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        public Optional<string> Username { get; set; }
        /// <summary>
        ///     Gets or sets the avatar.
        /// </summary>
        public Optional<Image?> Avatar { get; set; }
    }
}

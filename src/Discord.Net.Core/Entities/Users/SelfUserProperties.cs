namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify the <see cref="ISelfUser" /> with the specified changes.
    /// </summary>
    /// <example>
    ///     <code lang="c#">
    ///         await Context.Client.CurrentUser.ModifyAsync(x =&gt;
    ///         {
    ///             x.Avatar = new Image(File.OpenRead("avatar.jpg"));
    ///         });
    ///     </code>
    /// </example>
    /// <seealso cref="T:Discord.ISelfUser" />
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

namespace Discord
{
    /// <summary>
    /// Modify the current user with the specified arguments
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await Context.Client.CurrentUser.ModifyAsync(x =>
    /// {
    ///     x.Avatar = new Image(File.OpenRead("avatar.jpg"));
    /// });
    /// </code>
    /// </example>
    /// <seealso cref="ISelfUser"/>
    public class SelfUserProperties
    {
        /// <summary>
        /// Your username
        /// </summary>
        public Optional<string> Username { get; set; }
        /// <summary>
        /// Your avatar
        /// </summary>
        public Optional<Image?> Avatar { get; set; }
    }
}

namespace Discord
{
    /// <summary>
    /// Modify an IGuildChannel with the specified changes.
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await (Context.Channel as ITextChannel)?.ModifyAsync(x =>
    /// {
    ///     x.Name = "do-not-enter";
    /// });
    /// </code>
    /// </example>
    public class GuildChannelProperties
    {
        /// <summary>
        /// Set the channel to this name
        /// </summary>
        /// <remarks>
        /// When modifying an ITextChannel, the Name MUST be alphanumeric with dashes.
        /// It must match the following RegEx: [a-z0-9-_]{2,100}
        /// </remarks>
        /// <exception cref="Net.HttpException">A BadRequest will be thrown if the name does not match the above RegEx.</exception>
        public Optional<string> Name { get; set; }
        /// <summary>
        /// Move the channel to the following position. This is 0-based!
        /// </summary>
        public Optional<int> Position { get; set; }
    }
}

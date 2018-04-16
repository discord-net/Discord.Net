namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IGuildChannel" /> with the specified changes.
    /// </summary>
    /// <example>
    ///     <code lang="c#">
    ///         await (Context.Channel as ITextChannel)?.ModifyAsync(x =&gt;
    ///         {
    ///             x.Name = "do-not-enter";
    ///         });
    ///     </code>
    /// </example>
    public class GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the channel to this name.
        /// </summary>
        /// <remarks>
        ///     When modifying an <see cref="ITextChannel" />, the <see cref="Name" />
        ///     MUST be alphanumeric with dashes. It must match the following RegEx: [a-z0-9-_]{2,100}
        /// </remarks>
        /// <exception cref="Discord.Net.HttpException">
        /// A BadRequest will be thrown if the name does not match the above RegEx.
        /// </exception>
        public Optional<string> Name { get; set; }
        /// <summary>
        ///     Moves the channel to the following position. This is 0-based!
        /// </summary>
        public Optional<int> Position { get; set; }
        /// <summary>
        ///     Gets or sets the category for this channel.
        /// </summary>
        public Optional<ulong?> CategoryId { get; set; }
    }
}

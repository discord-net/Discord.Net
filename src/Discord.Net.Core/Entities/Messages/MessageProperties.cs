namespace Discord
{
    /// <summary>
    /// Modify a message with the specified parameters.
    /// </summary>
    /// <remarks>
    /// The content of a message can be cleared with String.Empty; if and only if an Embed is present.
    /// </remarks>
    /// <example>
    /// <code language="c#">
    /// var message = await ReplyAsync("abc");
    /// await message.ModifyAsync(x =>
    /// {
    ///     x.Content = "";
    ///     x.Embed = new EmbedBuilder()
    ///         .WithColor(new Color(40, 40, 120))
    ///         .WithAuthor(a => a.Name = "foxbot")
    ///         .WithTitle("Embed!")
    ///         .WithDescription("This is an embed.");
    /// });
    /// </code>
    /// </example>
    public class MessageProperties
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        /// <remarks>
        /// This must be less than 2000 characters.
        /// </remarks>
        public Optional<string> Content { get; set; }
        /// <summary>
        /// The embed the message should display
        /// </summary>
        public Optional<Embed> Embed { get; set; }
    }
}

namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="IUserMessage" /> with the specified changes.
    /// </summary>
    /// <remarks>
    ///     The content of a message can be cleared with <see cref="System.String.Empty"/> if and only if an 
    ///     <see cref="Discord.Embed"/> is present.
    /// </remarks>
    /// <seealso cref="IUserMessage.ModifyAsync"/>
    public class MessageProperties
    {
        /// <summary>
        ///     Gets or sets the content of the message.
        /// </summary>
        /// <remarks>
        ///     This must be less than the constant defined by <see cref="DiscordConfig.MaxMessageSize"/>.
        /// </remarks>
        public Optional<string> Content { get; set; }
        /// <summary>
        ///     Gets or sets the embed the message should display.
        /// </summary>
        public Optional<Embed> Embed { get; set; }
        /// <summary>
        ///     Gets or sets the flags of the message.
        /// </summary>
        /// <remarks>
        ///     Only <see cref="MessageFlags.SuppressEmbeds"/> can be set/unset and you need to be
        ///     the author of the message.
        /// </remarks>
        public Optional<MessageFlags?> Flags { get; set; }
        /// <summary>
        ///     Gets or sets the allowed mentions of the message.
        /// </summary>
        public Optional<AllowedMentions> AllowedMentions { get; set; }
    }
}

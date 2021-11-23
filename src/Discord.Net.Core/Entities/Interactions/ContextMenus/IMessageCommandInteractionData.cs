namespace Discord
{
    /// <summary>
    ///     Represents the data tied with the <see cref="IMessageCommandInteraction"/> interaction.
    /// </summary>
    public interface IMessageCommandInteractionData : IApplicationCommandInteractionData
    {
        /// <summary>
        ///     Gets the message associated with this message command.
        /// </summary>
        IMessage Message { get; }
    }
}

namespace Discord
{
    /// <summary>
    ///     Represents a User Command interaction.
    /// </summary>
    public interface IUserCommandInteraction : IApplicationCommandInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        new IUserCommandInteractionData Data { get; }
    }
}

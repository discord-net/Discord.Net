namespace Discord
{
    /// <summary>
    ///     Represents a slash command interaction.
    /// </summary>
    public interface ISlashCommandInteraction : IApplicationCommandInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        new IApplicationCommandInteractionData Data { get; }
    }
}

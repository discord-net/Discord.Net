namespace Discord
{
    /// <summary>
    ///     Represents a slash command interaction.
    /// </summary>
    public interface ISlashCommandInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        new IApplicationCommandInteractionData Data { get; }
    }
}

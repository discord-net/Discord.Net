namespace Discord
{
    /// <summary>
    ///     Represents an application command interaction.
    /// </summary>
    public interface IApplicationCommandInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data of the application command interaction
        /// </summary>
        new IApplicationCommandInteractionData Data { get; }
    }
}

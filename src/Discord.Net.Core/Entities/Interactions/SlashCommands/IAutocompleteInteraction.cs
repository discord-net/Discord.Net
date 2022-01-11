namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="InteractionType.ApplicationCommandAutocomplete"/>.
    /// </summary>
    public interface IAutocompleteInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the autocomplete data of this interaction.
        /// </summary>
        new IAutocompleteInteractionData Data { get; }
    }
}

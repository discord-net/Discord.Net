namespace Discord
{
    /// <summary>
    ///     Represents a message component on a message.
    /// </summary>
    public interface IMessageComponent
    {
        /// <summary>
        ///     Gets the <see cref="ComponentType"/> of this Message Component.
        /// </summary>
        ComponentType Type { get; }

        /// <summary>
        ///     Gets the custom id of the component if possible; otherwise <see langword="null"/>.
        /// </summary>
        string CustomId { get; }
    }
}

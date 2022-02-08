namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a generic <see cref="Modal"/> for use with the interaction service.
    /// </summary>
    public interface IModal
    {
        /// <summary>
        ///     Gets the modal's title.
        /// </summary>
        string Title { get; }
    }
}

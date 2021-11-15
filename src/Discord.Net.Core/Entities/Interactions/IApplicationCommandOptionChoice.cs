namespace Discord
{
    /// <summary>
    ///     Specifies choices for command group.
    /// </summary>
    public interface IApplicationCommandOptionChoice
    {
        /// <summary>
        ///     Gets the choice name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the value of the choice.
        /// </summary>
        object Value { get; }
    }
}

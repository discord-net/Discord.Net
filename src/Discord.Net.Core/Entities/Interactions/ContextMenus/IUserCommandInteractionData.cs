namespace Discord
{
    /// <summary>
    ///     Represents the data tied with the <see cref="IUserCommandInteraction"/> interaction.
    /// </summary>
    public interface IUserCommandInteractionData : IApplicationCommandInteractionData
    {
        /// <summary>
        ///     Gets the user who this command targets.
        /// </summary>
        IUser User { get; }
    }
}

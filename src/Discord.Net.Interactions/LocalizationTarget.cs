namespace Discord.Interactions
{
    /// <summary>
    ///     Resource targets for localization.
    /// </summary>
    public enum LocalizationTarget
    {
        /// <summary>
        ///     Target is a <see cref="IInteractionModuleBase"/> tagged with a <see cref="GroupAttribute"/>.
        /// </summary>
        Group,
        /// <summary>
        ///     Target is an application command method.
        /// </summary>
        Command,
        /// <summary>
        ///     Target is a Slash Command parameter.
        /// </summary>
        Parameter,
        /// <summary>
        ///     Target is a Slash Command parameter choice.
        /// </summary>
        Choice
    }
}

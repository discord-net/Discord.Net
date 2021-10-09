namespace Discord
{
    /// <summary>
    ///     ApplicationCommandType is enum of current valid Application Command Types: Slash, User, Message
    /// </summary>
    public enum ApplicationCommandType : byte
    {
        /// <summary>
        ///     ApplicationCommandType.Slash is Slash command type
        /// </summary>
        Slash = 1,

        /// <summary>
        ///     ApplicationCommandType.User is Context Menu User command type
        /// </summary>
        User = 2,

        /// <summary>
        ///     ApplicationCommandType.Message is Context Menu Message command type
        /// </summary>
        Message = 3
    }
}

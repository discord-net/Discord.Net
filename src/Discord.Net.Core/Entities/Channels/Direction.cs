namespace Discord
{
    /// <summary>
    ///     Specifies the direction of where message(s) should be gotten from.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        ///     The message(s) should be retrieved before a message.
        /// </summary>
        Before,
        /// <summary>
        ///     The message(s) should be retrieved after a message.
        /// </summary>
        After,
        /// <summary>
        ///     The message(s) should be retrieved around a message.
        /// </summary>
        Around
    }
}

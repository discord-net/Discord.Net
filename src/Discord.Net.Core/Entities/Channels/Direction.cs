namespace Discord
{
    /// <summary>
    ///     Specifies the direction of where message(s) should be gotten from.
    /// </summary>
    /// <remarks>
    ///     This enum is used to specify what direction should the message retrieval go.
    ///     <note type="important">
    ///         At the time of writing, <see cref="Around"/> is not yet implemented into 
    ///         <see cref="IMessageChannel.GetMessagesAsync"/>. Attempting to use the method with <see cref="Around"/>
    ///         as its direction will throw a <see cref="System.NotImplementedException"/>.
    ///     </note>
    /// </remarks>
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

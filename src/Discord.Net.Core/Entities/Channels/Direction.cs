namespace Discord
{
    /// <summary>
    ///     Specifies the direction of where entities (e.g. bans/messages) should be retrieved from.
    /// </summary>
    /// <remarks>
    ///     This enum is used to specify the direction for retrieving entities.
    ///     <note type="important">
    ///         At the time of writing, <see cref="Around"/> is not yet implemented into 
    ///         <see cref="IMessageChannel.GetMessagesAsync(int, CacheMode, RequestOptions)"/>.
    ///         Attempting to use the method with <see cref="Around"/> will throw
    ///         a <see cref="System.NotImplementedException"/>.
    ///     </note>
    /// </remarks>
    public enum Direction
    {
        /// <summary>
        ///     The entity(s) should be retrieved before an entity.
        /// </summary>
        Before,
        /// <summary>
        ///     The entity(s) should be retrieved after an entity.
        /// </summary>
        After,
        /// <summary>
        ///     The entity(s) should be retrieved around an entity.
        /// </summary>
        Around
    }
}

namespace Discord
{
    /// <summary>
    ///     Specifies the type of message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     The default message type.
        /// </summary>
        Default = 0,
        /// <summary>
        ///     The message when a recipient is added.
        /// </summary>
        RecipientAdd = 1,
        /// <summary>
        ///     The message when a recipient is removed.
        /// </summary>
        RecipientRemove = 2,
        /// <summary>
        ///     The message when a user is called.
        /// </summary>
        Call = 3,
        /// <summary>
        ///     The message when a channel name is changed.
        /// </summary>
        ChannelNameChange = 4,
        /// <summary>
        ///     The message when a channel icon is changed.
        /// </summary>
        ChannelIconChange = 5,
        /// <summary>
        ///     The message when another message is pinned.
        /// </summary>
        ChannelPinnedMessage = 6
    }
}

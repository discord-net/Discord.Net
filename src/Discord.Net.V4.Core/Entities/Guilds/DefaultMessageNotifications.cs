namespace Discord
{
    /// <summary>
    ///     Specifies the default message notification behavior the guild uses.
    /// </summary>
    public enum DefaultMessageNotifications
    {
        /// <summary>
        ///     By default, all messages will trigger notifications.
        /// </summary>
        AllMessages = 0,
        /// <summary>
        ///     By default, only mentions will trigger notifications.
        /// </summary>
        MentionsOnly = 1
    }
}

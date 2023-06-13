namespace Discord
{
    public enum TargetUserType
    {
        /// <summary>
        ///     The invite whose target user type is not defined.
        /// </summary>
        Undefined = 0,
        /// <summary>
        ///     The invite is for a Go Live stream.
        /// </summary>
        Stream = 1,
        /// <summary>
        ///     The invite is for embedded application.
        /// </summary>
        EmbeddedApplication = 2
    }
}

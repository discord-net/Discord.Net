namespace Discord
{
    /// <summary> Login state for clients </summary>
    public enum LoginState : byte
    {
        /// <summary> Logged out </summary>
        LoggedOut,
        /// <summary> Logging in </summary>
        LoggingIn,
        /// <summary> Logged in </summary>
        LoggedIn,
        /// <summary> Logging out </summary>
        LoggingOut
    }
}

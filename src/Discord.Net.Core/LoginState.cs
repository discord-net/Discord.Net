namespace Discord
{
    /// <summary> Specifies the state of the client's login status. </summary>
    public enum LoginState : byte
    {
        /// <summary> The client is currently logged out. </summary>
        LoggedOut,
        /// <summary> The client is currently logging in. </summary>
        LoggingIn,
        /// <summary> The client is currently logged in. </summary>
        LoggedIn,
        /// <summary> The client is currently logging out. </summary>
        LoggingOut
    }
}

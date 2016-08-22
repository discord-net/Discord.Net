namespace Discord
{
    /// <summary> Connection state for clients </summary> 
    public enum ConnectionState : byte
    {
        /// <summary> Not connected to Discord </summary>
        Disconnected,
        /// <summary> Currently connecting to Discord </summary>
        Connecting,
        /// <summary> Connected to Discord </summary>
        Connected,
        /// <summary> Disconnecting from Discord </summary>
        Disconnecting
    }
}

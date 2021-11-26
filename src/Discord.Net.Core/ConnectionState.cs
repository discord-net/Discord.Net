namespace Discord
{
    /// <summary> Specifies the connection state of a client. </summary>
    public enum ConnectionState : byte
    {
        /// <summary> The client has disconnected from Discord. </summary>
        Disconnected,
        /// <summary> The client is connecting to Discord. </summary>
        Connecting,
        /// <summary> The client has established a connection to Discord. </summary>
        Connected,
        /// <summary> The client is disconnecting from Discord. </summary>
        Disconnecting
    }
}

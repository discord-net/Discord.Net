namespace Discord
{
    /// <summary> Specifies the connection state of a client. </summary>
    public enum ConnectionState : byte
    {
        /// <summary> Represents that the client has disconnected from the WebSocket. </summary>
        Disconnected,
        /// <summary> Represents that the client is connecting to the WebSocket. </summary>
        Connecting,
        /// <summary> Represents that the client has established a connection to the WebSocket. </summary>
        Connected,
        /// <summary> Represents that the client is disconnecting from the WebSocket. </summary>
        Disconnecting
    }
}

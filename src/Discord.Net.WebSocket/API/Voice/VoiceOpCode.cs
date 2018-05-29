#pragma warning disable CS1591
namespace Discord.API.Voice
{
    internal enum VoiceOpCode : byte
    {
        /// <summary> C→S - Used to associate a connection with a token. </summary>
        Identify = 0,
        /// <summary> C→S - Used to specify configuration. </summary>
        SelectProtocol = 1,
        /// <summary> C←S - Used to notify that the voice connection was successful and informs the client of available protocols. </summary>
        Ready = 2,
        /// <summary> C→S - Used to keep the connection alive and measure latency. </summary>
        Heartbeat = 3,
        /// <summary> C←S - Used to provide an encryption key to the client. </summary>
        SessionDescription = 4,
        /// <summary> C↔S - Used to inform that a certain user is speaking. </summary>
        Speaking = 5,
        /// <summary> C←S - Used to reply to a client's heartbeat. </summary>
        HeartbeatAck = 6,
        /// <summary> C→S - Used to resume a connection. </summary>
        Resume = 7,
        /// <summary> C←S - Used to inform the client the heartbeat interval. </summary>
        Hello = 8,
        /// <summary> C←S - Used to acknowledge a resumed connection. </summary>
        Resumed = 9,
        /// <summary> C←S - Used to notify that a client has disconnected. </summary>
        ClientDisconnect = 13,
    }
}

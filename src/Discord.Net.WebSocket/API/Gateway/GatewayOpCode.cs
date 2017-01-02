#pragma warning disable CS1591
namespace Discord.API.Gateway
{
    internal enum GatewayOpCode : byte
    {
        /// <summary> C←S - Used to send most events. </summary>
        Dispatch = 0,
        /// <summary> C↔S - Used to keep the connection alive and measure latency. </summary>
        Heartbeat = 1,
        /// <summary> C→S - Used to associate a connection with a token and specify configuration. </summary>
        Identify = 2,
        /// <summary> C→S - Used to update client's status and current game id. </summary>
        StatusUpdate = 3,
        /// <summary> C→S - Used to join a particular voice channel. </summary>
        VoiceStateUpdate = 4,
        /// <summary> C→S - Used to ensure the guild's voice server is alive. </summary>
        VoiceServerPing = 5,
        /// <summary> C→S - Used to resume a connection after a redirect occurs. </summary>
        Resume = 6,
        /// <summary> C←S - Used to notify a client that they must reconnect to another gateway. </summary>
        Reconnect = 7,
        /// <summary> C→S - Used to request members that were withheld by large_threshold </summary>
        RequestGuildMembers = 8,
        /// <summary> C←S - Used to notify the client that their session has expired and cannot be resumed. </summary>
        InvalidSession = 9,
        /// <summary> C←S - Used to provide information to the client immediately on connection. </summary>
        Hello = 10,
        /// <summary> C←S - Used to reply to a client's heartbeat. </summary>
        HeartbeatAck = 11,
        /// <summary> C→S - Used to request presence updates from particular guilds. </summary>
        GuildSync = 12
    }
}

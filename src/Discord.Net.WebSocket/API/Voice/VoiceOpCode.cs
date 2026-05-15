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

        /// <summary> C←S - One or more clients have connected to the voice channel. </summary>
        ClientConnect = 11,

        /// <summary> C←S - Used to notify that a client has disconnected. </summary>
        ClientDisconnect = 13,

        /// <summary> C←S - Contains the flags of a user that connected to voice, also sent on initial connection for each existing user. </summary>
        ClientFlags = 18,

        /// <summary> C←S - Contains the platform type of a user that connected to voice, also sent on initial connection for each existing user. </summary>
        ClientPlatform = 20,

        /// <summary> C←S - A downgrade from the DAVE protocol is upcoming.</summary>
        DavePrepareTransition = 21,

        /// <summary> C←S - Execute a previously announced protocol transition.</summary>
        DaveExecuteTransition = 22,

        /// <summary> C→S - Acknowledge readiness previously announced transition.</summary>
        DaveTransitionReady = 23,

        /// <summary> C←S - A DAVE protocol version or group change is upcoming.</summary>
        DavePrepareEpoc = 24,

        /// <summary> C←S - Credential and public key for MLS external sender.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveMLSExternalSender = 25,

        /// <summary> C→S - MLS Key Package for pending group member.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveMLSKeyPackage = 26,

        /// <summary> C←S - MLS Proposals to be appended or revoked.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveMLSProposals = 27,

        /// <summary> C→S - MLS Commit with optional MLS Welcome messages.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveMLSCommitWelcome = 28,

        /// <summary> C←S - MLS Commit to be processed for upcoming transition.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveAnnounceCommitTransaction = 29,

        /// <summary> C←S - MLS Welcome to group for upcoming transition.</summary>
        /// <remarks>Binary message format only.</remarks>
        DaveMLSWelcome = 30,

        /// <summary> C←S - Flag invalid commit or welcome, request re-add.</summary>
        DaveMLSInvalidCommitWelcome = 31
    }
}

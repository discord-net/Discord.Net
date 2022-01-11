using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents generic op codes for voice disconnect.
    /// </summary>
    public enum VoiceCloseCode
    {
        /// <summary>
        ///     You sent an invalid opcode.
        /// </summary>
        UnknownOpcode = 4001,
        /// <summary>
        ///     You sent an invalid payload in your identifying to the Gateway.
        /// </summary>
        DecodeFailure = 4002,
        /// <summary>
        ///     You sent a payload before identifying with the Gateway.
        /// </summary>
        NotAuthenticated = 4003,
        /// <summary>
        ///     The token you sent in your identify payload is incorrect.
        /// </summary>
        AuthenticationFailed = 4004,
        /// <summary>
        ///     You sent more than one identify payload. Stahp.
        /// </summary>
        AlreadyAuthenticated = 4005,
        /// <summary>
        ///     Your session is no longer valid.
        /// </summary>
        SessionNolongerValid = 4006,
        /// <summary>
        ///     Your session has timed out.
        /// </summary>
        SessionTimeout = 4009,
        /// <summary>
        ///     We can't find the server you're trying to connect to.
        /// </summary>
        ServerNotFound = 4011,
        /// <summary>
        ///     We didn't recognize the protocol you sent.
        /// </summary>
        UnknownProtocol = 4012,
        /// <summary>
        ///     Channel was deleted, you were kicked, voice server changed, or the main gateway session was dropped. Should not reconnect.
        /// </summary>
        Disconnected = 4014,
        /// <summary>
        ///     The server crashed. Our bad! Try resuming.
        /// </summary>
        VoiceServerCrashed = 4015,
        /// <summary>
        ///     We didn't recognize your encryption.
        /// </summary>
        UnknownEncryptionMode = 4016,
    }
}

using System.Diagnostics;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based voice server.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceServer
    {
        /// <summary>
        ///     Gets the guild associated with the voice server.
        /// </summary>
        /// <returns>
        ///     A cached entity of the guild.
        /// </returns>
        public Cacheable<IGuild, ulong> Guild { get; }
        /// <summary>
        ///     Gets the endpoint URL of the voice server host.
        /// </summary>
        /// <returns>
        ///     An URL representing the voice server host.
        /// </returns>
        public string Endpoint { get; }
        /// <summary>
        /// 	Gets the voice connection token.
        /// </summary>
        /// <returns>
        /// 	A voice connection token.
        /// </returns>
        public string Token { get; }

        internal SocketVoiceServer(Cacheable<IGuild, ulong> guild, string endpoint, string token)
        {
            Guild = guild;
            Endpoint = endpoint;
            Token = token;
        }

        private string DebuggerDisplay => $"SocketVoiceServer ({Guild.Id})";
    }
}

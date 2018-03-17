using System.Diagnostics;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceServer
    {
        public ulong GuildId { get; private set; }
        public string Endpoint { get; private set; }
        public string Token { get; private set; }

        internal SocketVoiceServer(ulong guildId, string endpoint, string token)
        {
            GuildId = guildId;
            Endpoint = endpoint;
            Token = token;
        }

        private string DebuggerDisplay => $"SocketVoiceServer ({GuildId})";
    }
}

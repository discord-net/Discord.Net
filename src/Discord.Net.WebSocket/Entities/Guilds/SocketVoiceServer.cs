using System.Diagnostics;

namespace Discord.WebSocket.Entities.Guilds
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceServer
    {
        public ulong GuildId { get; private set; }
        public string Endpoint { get; private set; }
        public string Token { get; private set; }

        internal SocketVoiceServer(ulong GuildId, string Endpoint, string Token)
        {
            this.GuildId = GuildId;
            this.Endpoint = Endpoint;
            this.Token = Token;
        }
    }
}

using System.Diagnostics;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketVoiceServer
    {
        public Cacheable<IGuild, ulong> Guild { get; private set; }
        public string Endpoint { get; private set; }
        public string Token { get; private set; }

        internal SocketVoiceServer(Cacheable<IGuild, ulong> guild, string endpoint, string token)
        {
            Guild = guild;
            Endpoint = endpoint;
            Token = token;
        }

        private string DebuggerDisplay => $"SocketVoiceServer ({Guild.Id})";
    }
}

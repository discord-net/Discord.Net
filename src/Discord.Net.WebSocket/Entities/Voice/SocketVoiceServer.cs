using System.Diagnostics;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class SocketVoiceServer
    {
        internal SocketVoiceServer(Cacheable<IGuild, ulong> guild, string endpoint, string token)
        {
            Guild = guild;
            Endpoint = endpoint;
            Token = token;
        }

        public Cacheable<IGuild, ulong> Guild { get; }
        public string Endpoint { get; }
        public string Token { get; }

        private string DebuggerDisplay => $"SocketVoiceServer ({Guild.Id})";
    }
}

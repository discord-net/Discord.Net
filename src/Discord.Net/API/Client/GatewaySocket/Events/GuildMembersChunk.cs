using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class GuildMembersChunkEvent
    {
        [JsonProperty("members")]
        public Member[] Members;
    }
}

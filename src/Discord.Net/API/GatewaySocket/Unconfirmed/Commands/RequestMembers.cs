using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestMembersCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.RequestGuildMembers;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("guild_id")]
        public ulong[] GuildId { get; set; }
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}

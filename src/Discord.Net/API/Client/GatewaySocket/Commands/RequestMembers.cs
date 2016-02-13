using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestMembersCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCodes.RequestGuildMembers;
        object IWebSocketMessage.Payload => this;
        bool IWebSocketMessage.IsPrivate => false;

        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] GuildId { get; set; }
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}

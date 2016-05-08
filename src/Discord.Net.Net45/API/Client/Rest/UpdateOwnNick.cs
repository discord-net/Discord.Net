using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateOwnNick : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/@me/nick";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; set; }

        [JsonProperty("nick")]
        public string Nickname { get; set; }

        public UpdateOwnNick(ulong guildId, string nickname)
        {
            GuildId = guildId;
            Nickname = nickname;
        }
    }
}

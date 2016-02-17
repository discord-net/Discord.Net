using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/channels";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }

        public CreateChannelRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

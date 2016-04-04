using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateGuildIntegrationRequest : IRestRequest<Integration>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/integrations";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }

        [JsonProperty("id")]
        public ulong IntegrationId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }

        public CreateGuildIntegrationRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

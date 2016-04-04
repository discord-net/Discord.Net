using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyGuildIntegrationRequest : IRestRequest<Integration>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/integrations/{IntegrationId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }        
        public ulong IntegrationId { get; }

        [JsonProperty("expire_behavior")]
        public int ExpireBehavior { get; set; }
        [JsonProperty("expire_grace_period")]
        public int ExpireGracePeriod { get; set; }
        [JsonProperty("enable_emoticons")]
        public bool EnableEmoticons { get; set; }

        public ModifyGuildIntegrationRequest(ulong guildId, ulong integrationId)
        {
            GuildId = guildId;
            IntegrationId = integrationId;
        }
    }
}

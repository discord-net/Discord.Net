using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class SyncGuildIntegrationRequest : IRestRequest<Integration>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/integrations/{IntegrationId}/sync";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }        
        public ulong IntegrationId { get; }

        public SyncGuildIntegrationRequest(ulong guildId, ulong integrationId)
        {
            GuildId = guildId;
            IntegrationId = integrationId;
        }
    }
}

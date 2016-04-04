using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteGuildIntegrationRequest : IRestRequest<Integration>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/integrations/{IntegrationId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }        
        public ulong IntegrationId { get; }

        public DeleteGuildIntegrationRequest(ulong guildId, ulong integrationId)
        {
            GuildId = guildId;
            IntegrationId = integrationId;
        }
    }
}

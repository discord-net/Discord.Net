using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildIntegrationsRequest : IRestRequest<Integration[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/integrations";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildIntegrationsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

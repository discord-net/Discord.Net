using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildVoiceRegionsRequest : IRestRequest<GetVoiceRegionsResponse[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/regions";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildVoiceRegionsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

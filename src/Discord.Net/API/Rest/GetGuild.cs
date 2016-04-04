using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildBansRequest : IRestRequest<User[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildBansRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

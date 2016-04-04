using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildEmbedRequest : IRestRequest<GuildEmbed>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/embed";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public GetGuildEmbedRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

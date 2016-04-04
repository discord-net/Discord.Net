using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetGuildMemberRequest : IRestRequest<GuildMember>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/{UserId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        public GetGuildMemberRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}

using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class RemoveGuildMemberRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/members/{UserId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        public RemoveGuildMemberRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}

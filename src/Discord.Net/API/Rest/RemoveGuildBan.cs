using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class RemoveGuildBanRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans/{UserId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        public RemoveGuildBanRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}

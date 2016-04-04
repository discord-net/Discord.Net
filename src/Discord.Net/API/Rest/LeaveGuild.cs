using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class LeaveGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"users/@me/guilds/{GuildId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public LeaveGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

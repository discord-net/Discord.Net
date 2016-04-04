using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }

        public DeleteGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

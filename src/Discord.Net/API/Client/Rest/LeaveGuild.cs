using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class LeaveGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/guilds/{GuildId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }

        public LeaveGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

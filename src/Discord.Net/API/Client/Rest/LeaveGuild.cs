using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LeaveGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"users/@me/guilds/{GuildId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }

        public LeaveGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

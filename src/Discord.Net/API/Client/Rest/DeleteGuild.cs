using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeleteGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"guilds/{GuildId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }

        public DeleteGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

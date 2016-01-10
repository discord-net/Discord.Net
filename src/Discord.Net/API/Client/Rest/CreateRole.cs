using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CreateRoleRequest : IRestRequest<Role>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/roles";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; set; }

        public CreateRoleRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}

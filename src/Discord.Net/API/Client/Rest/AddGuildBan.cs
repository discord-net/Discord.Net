using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AddGuildBanRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans/{UserId}?delete-message-days={PruneDays}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }

        public int PruneDays { get; set; } = 0;

        public AddGuildBanRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}

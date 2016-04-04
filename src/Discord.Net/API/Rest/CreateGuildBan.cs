using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateGuildBanRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans/{UserId}";
        object IRestRequest.Payload => null;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        [JsonProperty("delete-message-days")]
        public int PruneDays { get; set; } = 0;

        public CreateGuildBanRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
}

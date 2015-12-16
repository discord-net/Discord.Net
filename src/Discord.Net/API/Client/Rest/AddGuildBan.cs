using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AddGuildBanRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/bans/{UserId}?delete-message-days={PruneDays}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }
        public ulong UserId { get; }

        public int PruneDays { get; set; } = 0;

        public AddGuildBanRequest(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = UserId;
        }
    }
}

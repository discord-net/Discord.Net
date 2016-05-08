using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateGuildBanParams
    {
        [JsonProperty("delete-message-days")]
        public int PruneDays { get; set; } = 0;
    }
}

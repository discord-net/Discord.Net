using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateGuildBanParams
    {
        [JsonProperty("delete-message-days")]
        public Optional<int> PruneDays { get; set; }
    }
}

using Newtonsoft.Json;

namespace Discord.API
{
    internal class ModalInteractionData : IDiscordInteractionData
    {
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("components")]
        public API.ActionRowComponent[] Components { get; set; }
    }
}

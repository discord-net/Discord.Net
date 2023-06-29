using Newtonsoft.Json;

namespace Discord.API
{
    internal class ModalInteractionData : IDiscordInteractionData
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; }

        [JsonPropertyName("components")]
        public API.ActionRowComponent[] Components { get; set; }
    }
}

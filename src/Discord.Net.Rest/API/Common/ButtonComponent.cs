using Newtonsoft.Json;

namespace Discord.API
{
    internal class ButtonComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("style")]
        public ButtonStyle Style { get; set; }

        [JsonProperty("label")]
        public Optional<string> Label { get; set; }

        [JsonProperty("emoji")]
        public Optional<Emoji> Emote { get; set; }

        [JsonProperty("custom_id")]
        public Optional<string> CustomId { get; set; }

        [JsonProperty("url")]
        public Optional<string> Url { get; set; }

        [JsonProperty("disabled")]
        public Optional<bool> Disabled { get; set; }

        [JsonProperty("sku_id")]
        public Optional<ulong> SkuId { get; set; }

        public ButtonComponent() { }

        public ButtonComponent(Discord.ButtonComponent c)
        {
            Type = c.Type;
            Style = c.Style;
            Label = c.Label;
            CustomId = c.CustomId;
            Url = c.Url;
            Disabled = c.IsDisabled;
            SkuId = c.SkuId ?? Optional<ulong>.Unspecified;

            if (c.Emote != null)
            {
                if (c.Emote is Emote e)
                {
                    Emote = new Emoji
                    {
                        Name = e.Name,
                        Animated = e.Animated,
                        Id = e.Id
                    };
                }
                else
                {
                    Emote = new Emoji
                    {
                        Name = c.Emote.Name
                    };
                }
            }
        }

        [JsonIgnore]
        string IMessageComponent.CustomId => CustomId.GetValueOrDefault();
    }
}

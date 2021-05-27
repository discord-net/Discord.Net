using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class ButtonComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; } = ComponentType.Button;

        [JsonProperty("style")]
        public ButtonStyle Style { get; }

        [JsonProperty("label")]
        public string Label { get; }

        [JsonProperty("emoji")]
        public IEmote Emote { get; }

        [JsonProperty("custom_id")]
        public string CustomId { get; }

        [JsonProperty("url")]
        public string Url { get; }

        [JsonProperty("disabled")]
        public bool Disabled { get; }

        internal ButtonComponent(ButtonStyle style, string label, IEmote emote, string customId, string url, bool disabled)
        {
            this.Style = style;
            this.Label = label;
            this.Emote = emote;
            this.CustomId = customId;
            this.Url = url;
            this.Disabled = disabled;
        }
    }
}

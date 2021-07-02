using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public ButtonComponent() { }

        public ButtonComponent(Discord.ButtonComponent c)
        {
            this.Type = c.Type;
            this.Style = c.Style;
            this.Label = c.Label;
            this.CustomId = c.CustomId;
            this.Url = c.Url;
            this.Disabled = c.Disabled;

            if (c.Emote != null)
            {
                if (c.Emote is Emote e)
                {
                    this.Emote = new Emoji()
                    {
                        Name = e.Name,
                        Animated = e.Animated,
                        Id = e.Id,
                    };
                }
                else
                {
                    this.Emote = new Emoji()
                    {
                        Name = c.Emote.Name
                    };
                }
            }
        }
    }
}

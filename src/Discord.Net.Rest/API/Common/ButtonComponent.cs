using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ButtonComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("style")]
        public ButtonStyle Style { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("emoji")]
        public Emoji Emote { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }


        public ButtonComponent() { }

        public ButtonComponent(Discord.ButtonComponent c)
        {
            this.Type = c.Type;
            this.Style = c.Style;
            this.Label = c.Label;
            this.CustomId = c.CustomId;
            this.Url = c.Url;
            this.Disabled = c.Disabled;

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

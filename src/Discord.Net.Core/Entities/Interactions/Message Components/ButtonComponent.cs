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
        public ComponentType Type { get; } = ComponentType.Button;

        public ButtonStyle Style { get; }

        public string Label { get; }

        public IEmote Emote { get; }

        public string CustomId { get; }

        public string Url { get; }

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

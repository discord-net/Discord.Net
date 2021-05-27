using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class ActionRowComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; } = ComponentType.ActionRow;

        [JsonProperty("components")]
        public IReadOnlyCollection<IMessageComponent> Components { get; internal set; }

        internal ActionRowComponent() { }
        internal ActionRowComponent(IReadOnlyCollection<IMessageComponent> components)
        {
            this.Components = components;
        }
    }
}

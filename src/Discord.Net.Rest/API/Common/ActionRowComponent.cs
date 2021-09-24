using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ActionRowComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("components")]
        public IMessageComponent[] Components { get; set; }

        internal ActionRowComponent() { }
        internal ActionRowComponent(Discord.ActionRowComponent c)
        {
            Type = c.Type;
            Components = c.Components?.Select<IMessageComponent, IMessageComponent>(x =>
            {
                return x.Type switch
                {
                    ComponentType.Button => new ButtonComponent(x as Discord.ButtonComponent),
                    ComponentType.SelectMenu => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                    _ => null,
                };
            }).ToArray();
        }

        [JsonIgnore]
        string IMessageComponent.CustomId => null;
    }
}

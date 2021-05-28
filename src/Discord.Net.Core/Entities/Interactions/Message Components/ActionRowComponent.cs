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
        public ComponentType Type { get; } = ComponentType.ActionRow;

        public IReadOnlyCollection<ButtonComponent> Components { get; internal set; }

        internal ActionRowComponent() { }
        internal ActionRowComponent(List<ButtonComponent> components)
        {
            this.Components = components;
        }
    }
}

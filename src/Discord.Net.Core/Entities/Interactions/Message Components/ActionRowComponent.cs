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

        public IReadOnlyCollection<IMessageComponent> Components { get; internal set; }

        internal ActionRowComponent() { }
        internal ActionRowComponent(IReadOnlyCollection<IMessageComponent> components)
        {
            this.Components = components;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class MessageComponent
    {
        public IReadOnlyCollection<IMessageComponent> Components { get; }

        internal MessageComponent(List<ActionRowComponent> components)
        {
            this.Components = components;
        }

        internal static MessageComponent Empty
            => new MessageComponent(new List<ActionRowComponent>());

        internal IMessageComponent[] ToModel()
            => this.Components.ToArray();
    }
}

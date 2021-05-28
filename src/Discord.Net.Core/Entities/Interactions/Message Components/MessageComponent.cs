using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a component object used to send components with messages.
    /// </summary>
    public class MessageComponent
    {
        /// <summary>
        ///     The components to be used in a message.
        /// </summary>
        public IReadOnlyCollection<ActionRowComponent> Components { get; }

        internal MessageComponent(List<ActionRowComponent> components)
        {
            this.Components = components;
        }

        /// <summary>
        ///     Returns a empty <see cref="MessageComponent"/>.
        /// </summary>
        internal static MessageComponent Empty
            => new MessageComponent(new List<ActionRowComponent>());
    }
}

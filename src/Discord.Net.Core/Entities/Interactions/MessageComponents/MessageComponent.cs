using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a component object used to send components with messages.
    /// </summary>
    public class MessageComponent
    {
        /// <summary>
        ///     Gets the components to be used in a message.
        /// </summary>
        public IReadOnlyCollection<ActionRowComponent> Components { get; }

        internal MessageComponent(List<ActionRowComponent> components)
        {
            Components = components;
        }

        /// <summary>
        ///     Returns a empty <see cref="MessageComponent"/>.
        /// </summary>
        internal static MessageComponent Empty
            => new MessageComponent(new List<ActionRowComponent>());
    }
}

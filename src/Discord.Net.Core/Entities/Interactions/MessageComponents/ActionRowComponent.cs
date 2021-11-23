using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="IMessageComponent"/> Row for child components to live in.
    /// </summary>
    public class ActionRowComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type => ComponentType.ActionRow;

        /// <summary>
        ///     Gets the child components in this row.
        /// </summary>
        public IReadOnlyCollection<IMessageComponent> Components { get; internal set; }

        internal ActionRowComponent() { }

        internal ActionRowComponent(List<IMessageComponent> components)
        {
            Components = components;
        }

        string IMessageComponent.CustomId => null;
    }
}

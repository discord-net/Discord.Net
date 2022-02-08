using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a component object used in <see cref="Modal"/>s.
    /// </summary>
    public class ModalComponent
    {
        /// <summary>
        ///     Gets the components to be used in a modal.
        /// </summary>
        public IReadOnlyCollection<ActionRowComponent> Components { get; }

        internal ModalComponent(List<ActionRowComponent> components)
        {
            Components = components;
        }
    }
}

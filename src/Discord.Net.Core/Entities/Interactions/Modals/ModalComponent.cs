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
        public IReadOnlyCollection<IMessageComponent> Components { get; }

        internal ModalComponent(List<IMessageComponent> components)
        {
            Components = components;
        }
    }
}

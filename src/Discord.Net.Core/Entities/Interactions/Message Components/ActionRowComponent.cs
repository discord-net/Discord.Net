using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="IMessageComponent"/> Row for child components to live in. 
    /// </summary>
    public class ActionRowComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type { get; } = ComponentType.ActionRow;

        /// <summary>
        ///     The child components in this row.
        /// </summary>
        public IReadOnlyCollection<ButtonComponent> Components { get; internal set; }

        internal ActionRowComponent() { }
        internal ActionRowComponent(List<ButtonComponent> components)
        {
            this.Components = components;
        }
    }
}

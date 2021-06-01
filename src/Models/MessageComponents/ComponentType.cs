using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the component type.
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        ///     A container for other components.
        /// </summary>
        ActionRow = 1,

        /// <summary>
        ///     A clickable button.
        /// </summary>
        Button = 2,
    }
}

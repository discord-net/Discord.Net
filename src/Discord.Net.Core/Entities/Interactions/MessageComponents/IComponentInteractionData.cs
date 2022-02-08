using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the data sent with the <see cref="IComponentInteraction"/>.
    /// </summary>
    public interface IComponentInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the component's Custom Id that was clicked.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the type of the component clicked.
        /// </summary>
        ComponentType Type { get; }

        /// <summary>
        ///     Gets the value(s) of a <see cref="SelectMenuComponent"/> interaction response.
        /// </summary>
        IReadOnlyCollection<string> Values { get; }

        /// <summary>
        ///     Gets the value of a <see cref="TextInputComponent"/> interaction response.
        /// </summary>
        public string Value { get; }
    }
}

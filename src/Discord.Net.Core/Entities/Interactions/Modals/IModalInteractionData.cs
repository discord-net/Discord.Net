using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the data sent with the <see cref="IModalInteraction"/>.
    /// </summary>
    public interface IModalInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the <see cref="Modal"/>'s Custom Id.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the <see cref="Modal"/> components submitted by the user.
        /// </summary>
        IReadOnlyCollection<IComponentInteractionData> Components { get; }
    }
}

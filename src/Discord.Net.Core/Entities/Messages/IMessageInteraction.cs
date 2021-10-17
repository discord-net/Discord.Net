using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a partial <see cref="IDiscordInteraction"/> within a message.
    /// </summary>
    public interface IMessageInteraction
    {
        /// <summary>
        ///     Gets the snowflake id of the interaction.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        ///     Gets the type of the interaction.
        /// </summary>
        InteractionType Type { get; }

        /// <summary>
        ///     Gets the name of the application command used.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the <seealso cref="IUser"/> who invoked the interaction.
        /// </summary>
        IUser User { get; }
    }
}

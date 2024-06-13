using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an application command interaction.
    /// </summary>
    public interface IApplicationCommandInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data of the application command interaction
        /// </summary>
        new IApplicationCommandInteractionData Data { get; }
    }
}

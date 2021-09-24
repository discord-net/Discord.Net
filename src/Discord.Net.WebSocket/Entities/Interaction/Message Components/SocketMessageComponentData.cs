using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data sent with a <see cref="InteractionType.MessageComponent"/>.
    /// </summary>
    public class SocketMessageComponentData
    {
        /// <summary>
        ///     The components Custom Id that was clicked
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     The type of the component clicked
        /// </summary>
        public ComponentType Type { get; }

        /// <summary>
        ///     The value(s) of a <see cref="SelectMenuComponent"/> interaction response.
        /// </summary>
        public IReadOnlyCollection<string> Values { get; }

        internal SocketMessageComponentData(Model model)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
        }
    }
}

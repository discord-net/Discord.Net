using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
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

        internal SocketMessageComponentData(Model model)
        {
            this.CustomId = model.CustomId;
            this.Type = model.ComponentType;
        }
    }
}

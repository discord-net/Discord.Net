using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents data for a <see cref="RestMessageComponent"/>.
    /// </summary>
    public class RestMessageComponentData : IComponentInteractionData, IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the components Custom Id that was clicked.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the type of the component clicked.
        /// </summary>
        public ComponentType Type { get; }

        /// <summary>
        ///     Gets the value(s) of a <see cref="SelectMenuComponent"/> interaction response.
        /// </summary>
        public IReadOnlyCollection<string> Values { get; }

        internal RestMessageComponentData(Model model)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
        }
    }
}

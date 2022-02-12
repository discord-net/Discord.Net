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
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <inheritdoc/>
        public ComponentType Type { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> Values { get; }

        /// <inheritdoc/>
        public string Value { get; }

        internal RestMessageComponentData(Model model)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
        }

        internal RestMessageComponentData(IMessageComponent component)
        {
            CustomId = component.CustomId;
            Type = component.Type;

            if (component is API.TextInputComponent textInput)
                Value = textInput.Value.Value;

            if (component is API.SelectMenuComponent select)
                Values = select.Values.Value;
        }
    }
}

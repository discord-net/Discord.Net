using System.Collections.Generic;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data sent with a <see cref="InteractionType.MessageComponent"/>.
    /// </summary>
    public class SocketMessageComponentData : IComponentInteractionData
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

        /// <summary>
        ///     Gets the value of a <see cref="TextInputComponent"/> interaction response.
        /// </summary>
        public string Value { get; }

        internal SocketMessageComponentData(Model model)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
            Value = model.Value.GetValueOrDefault();
        }

        internal SocketMessageComponentData(IMessageComponent component)
        {
            CustomId = component.CustomId;
            Type = component.Type;

            Value = component.Type == ComponentType.TextInput
                ? (component as API.TextInputComponent).Value.Value
                : null;

            Values = component.Type == ComponentType.SelectMenu
                ? (component as API.SelectMenuComponent).Values.Value
                : null;
        }
    }
}

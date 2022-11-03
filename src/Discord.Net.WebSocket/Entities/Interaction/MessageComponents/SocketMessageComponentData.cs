using Discord.Utils;
using System.Linq;
using System.Collections.Generic;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data sent with a <see cref="InteractionType.MessageComponent"/>.
    /// </summary>
    public class SocketMessageComponentData : IComponentInteractionData
    {
        /// <inheritdoc />
        public string CustomId { get; }

        /// <inheritdoc />
        public ComponentType Type { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Values { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IChannel> Channels { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IUser> Users { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IRole> Roles { get; }

        /// <inheritdoc />
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

            if (component is API.SelectMenuComponent select)
            {
                Values = select.Values.GetValueOrDefault(null);
            }
        }
    }
}

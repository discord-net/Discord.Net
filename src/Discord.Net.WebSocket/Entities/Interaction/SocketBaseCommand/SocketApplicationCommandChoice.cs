using Model = Discord.API.ApplicationCommandOptionChoice;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a choice for a <see cref="SocketApplicationCommandOption"/>.
    /// </summary>
    public class SocketApplicationCommandChoice : IApplicationCommandOptionChoice
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public object Value { get; private set; }

        internal SocketApplicationCommandChoice() { }
        internal static SocketApplicationCommandChoice Create(Model model)
        {
            var entity = new SocketApplicationCommandChoice();
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            Value = model.Value;
        }
    }
}

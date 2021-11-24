using Model = Discord.API.ApplicationCommandOptionChoice;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IApplicationCommandOptionChoice"/>.
    /// </summary>
    public class RestApplicationCommandChoice : IApplicationCommandOptionChoice
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public object Value { get; }

        internal RestApplicationCommandChoice(Model model)
        {
            Name = model.Name;
            Value = model.Value;
        }
    }
}

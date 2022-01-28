using Discord.Interactions.Builders;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the parameter info class for <see cref="ComponentCommandInfo"/> commands.
    /// </summary>
    public class ComponentCommandParameterInfo : CommandParameterInfo
    {
        /// <summary>
        ///     Gets the <see cref="TypeReader"/> that will be used to convert a message component value into
        ///     <see cref="CommandParameterInfo.ParameterType"/>.
        /// </summary>
        public CompTypeConverter TypeReader { get; }

        internal ComponentCommandParameterInfo(ComponentCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            TypeReader = builder.TypeReader;
        }
    }
}

using Discord.Interactions.Builders;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the parameter info class for <see cref="ComponentCommandInfo"/> commands.
    /// </summary>
    public class ComponentCommandParameterInfo : CommandParameterInfo
    {
        /// <summary>
        ///     Gets the <see cref="ComponentTypeConverter"/> that will be used to convert a message component value into
        ///     <see cref="CommandParameterInfo.ParameterType"/>, if  <see cref="IsRouteSegmentParameter"/> is false.
        /// </summary>
        public ComponentTypeConverter TypeConverter { get; }

        /// <summary>
        ///     Gets the <see cref="TypeReader"/> that will be used to convert a CustomId segment value into
        ///     <see cref="CommandParameterInfo.ParameterType"/>, if <see cref="IsRouteSegmentParameter"/> is <see langword="true"/>.
        /// </summary>
        public TypeReader TypeReader { get; }

        /// <summary>
        ///     Gets whether this parameter is a CustomId segment or a component value parameter.
        /// </summary>
        public bool IsRouteSegmentParameter { get; }

        internal ComponentCommandParameterInfo(ComponentCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            TypeReader = builder.TypeReader;
            IsRouteSegmentParameter = builder.IsRouteSegmentParameter;
        }
    }
}

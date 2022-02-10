using Discord.Interactions.Builders;
using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the parameter info class for <see cref="ComponentCommandInfo"/> commands.
    /// </summary>
    public class ComponentCommandParameterInfo : CommandParameterInfo
    {
        /// <summary>
        ///     Gets the <see cref="ComponentTypeConverter"/> that will be used to convert a message component value into
        ///     <see cref="CommandParameterInfo.ParameterType"/>.
        /// </summary>
        public ComponentTypeConverter TypeConverter { get; }
        public TypeReader TypeReader { get; }
        public bool IsRouteSegmentParameter { get; }

        internal ComponentCommandParameterInfo(ComponentCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            TypeConverter = builder.TypeConverter;
            TypeReader = builder.TypeReader;
            IsRouteSegmentParameter = builder.IsRouteSegmentParameter;
        }
    }
}

using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ComponentCommandParameterInfo"/>.
    /// </summary>
    public class ComponentCommandParameterBuilder : ParameterBuilder<ComponentCommandParameterInfo, ComponentCommandParameterBuilder>
    {
        /// <summary>
        ///     Get the <see cref="ComponentTypeConverter"/> assigned to this parameter, if <see cref="IsRouteSegmentParameter"/> is <see langword="false"/>.
        /// </summary>
        public ComponentTypeConverter TypeConverter { get; private set; }

        /// <summary>
        ///     Get the <see cref="Discord.Interactions.TypeReader"/> assigned to this parameter, if <see cref="IsRouteSegmentParameter"/> is <see langword="true"/>.
        /// </summary>
        public TypeReader TypeReader { get; private set; }

        /// <summary>
        ///     Gets whether this parameter is a CustomId segment or a Component value parameter.
        /// </summary>
        public bool IsRouteSegmentParameter { get; private set; }

        /// <inheritdoc/>
        protected override ComponentCommandParameterBuilder Instance => this;

        internal ComponentCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        /// <summary>
        ///     Initializes a new <see cref="ComponentCommandParameterBuilder"/>.
        /// </summary>
        /// <param name="command">Parent command of this parameter.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="type">Type of this parameter.</param>
        public ComponentCommandParameterBuilder(ICommandBuilder command, string name, Type type) : base(command, name, type) { }

        /// <inheritdoc/>
        public override ComponentCommandParameterBuilder SetParameterType(Type type) => SetParameterType(type, null);

        /// <summary>
        ///     Sets <see cref="ParameterBuilder{TInfo, TBuilder}.ParameterType"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="ParameterBuilder{TInfo, TBuilder}.ParameterType"/>.</param>
        /// <param name="services">Service container to be used to resolve the dependencies of this parameters <see cref="Interactions.TypeConverter"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ComponentCommandParameterBuilder SetParameterType(Type type, IServiceProvider services)
        {
            base.SetParameterType(type);

            if (IsRouteSegmentParameter)
                TypeReader = Command.Module.InteractionService.GetTypeReader(type);
            else
                TypeConverter = Command.Module.InteractionService.GetComponentTypeConverter(ParameterType, services);

            return this;
        }

        /// <summary>
        ///     Sets <see cref="IsRouteSegmentParameter"/>.
        /// </summary>
        /// <param name="isRouteSegment">New value of the <see cref="IsRouteSegmentParameter"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ComponentCommandParameterBuilder SetIsRouteSegment(bool isRouteSegment)
        {
            IsRouteSegmentParameter = isRouteSegment;
            return this;
        }

        internal override ComponentCommandParameterInfo Build(ICommandInfo command)
            => new(this, command);
    }
}

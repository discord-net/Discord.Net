using System;

namespace Discord.Interactions.Builders
{
    public class ComponentCommandParameterBuilder : ParameterBuilder<ComponentCommandParameterInfo, ComponentCommandParameterBuilder>
    {
        public ComponentTypeConverter TypeConverter { get; private set; }
        public TypeReader TypeReader { get; private set; }
        public bool IsRouteSegmentParameter { get; private set; }
        protected override ComponentCommandParameterBuilder Instance => this;

        public ComponentCommandParameterBuilder(ICommandBuilder command) : base(command) { }

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
        public ComponentCommandParameterBuilder SetParameterType(Type type, IServiceProvider services = null)
        {
            base.SetParameterType(type);

            if (IsRouteSegmentParameter)
                TypeReader = Command.Module.InteractionService.GetTypeReader(type);
            else
                TypeConverter = Command.Module.InteractionService.GetComponentTypeConverter(ParameterType, services);

            return this;
        }

        public ComponentCommandParameterBuilder SetAsRouteSegment(bool isRouteSegment)
        {
            IsRouteSegmentParameter = isRouteSegment;
            return this;
        }

        internal override ComponentCommandParameterInfo Build(ICommandInfo command)
            => new(this, command);
    }
}

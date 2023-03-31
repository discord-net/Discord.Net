using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ComponentCommandInfo"/>.
    /// </summary>
    public sealed class ComponentCommandBuilder : CommandBuilder<ComponentCommandInfo, ComponentCommandBuilder, ComponentCommandParameterBuilder>
    {
        protected override ComponentCommandBuilder Instance => this;

        internal ComponentCommandBuilder(ModuleBuilder module) : base(module) { }

        /// <summary>
        ///     Initializes a new <see cref="ComponentBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public ComponentCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="CommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override ComponentCommandBuilder AddParameter(Action<ComponentCommandParameterBuilder> configure)
        {
            var parameter = new ComponentCommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override ComponentCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new ComponentCommandInfo(this, module, commandService);
    }
}

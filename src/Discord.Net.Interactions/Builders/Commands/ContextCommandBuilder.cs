using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ContextCommandInfo"/>.
    /// </summary>
    public sealed class ContextCommandBuilder : CommandBuilder<ContextCommandInfo, ContextCommandBuilder, CommandParameterBuilder>
    {
        protected override ContextCommandBuilder Instance => this;

        /// <summary>
        ///     Gets the type of this command.
        /// </summary>
        public ApplicationCommandType CommandType { get; set; }

        /// <summary>
        ///     Gets the default permission of this command.
        /// </summary>
        public bool DefaultPermission { get; set; } = true;

        internal ContextCommandBuilder (ModuleBuilder module) : base(module) { }

        /// <summary>
        ///     Initializes a new <see cref="ContextCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public ContextCommandBuilder (ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Sets <see cref="CommandType"/>.
        /// </summary>
        /// <param name="commandType">New value of the <see cref="CommandType"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder SetType (ApplicationCommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultPermission"/>.
        /// </summary>
        /// <param name="defaultPermision">New value of the <see cref="DefaultPermission"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ContextCommandBuilder SetDefaultPermission (bool defaultPermision)
        {
            DefaultPermission = defaultPermision;
            return this;
        }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="CommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override ContextCommandBuilder AddParameter (Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override ContextCommandInfo Build (ModuleInfo module, InteractionService commandService) =>
            ContextCommandInfo.Create(this, module, commandService);
    }
}

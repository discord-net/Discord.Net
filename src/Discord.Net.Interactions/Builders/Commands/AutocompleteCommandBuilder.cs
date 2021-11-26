using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="AutocompleteCommandInfo"/>.
    /// </summary>
    public sealed class AutocompleteCommandBuilder : CommandBuilder<AutocompleteCommandInfo, AutocompleteCommandBuilder, CommandParameterBuilder>
    {
        /// <summary>
        ///     Gets the name of the target parameter.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        ///     Gets the name of the target command.
        /// </summary>
        public string CommandName { get; set; }

        protected override AutocompleteCommandBuilder Instance => this;

        internal AutocompleteCommandBuilder(ModuleBuilder module) : base(module) { }

        /// <summary>
        ///     Initializes a new <see cref="AutocompleteCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public AutocompleteCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Sets <see cref="ParameterName"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="ParameterName"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public AutocompleteCommandBuilder WithParameterName(string name)
        {
            ParameterName = name;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="CommandName"/>.
        /// </summary>
        /// <param name="name">New value of the <see cref="CommandName"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public AutocompleteCommandBuilder WithCommandName(string name)
        {
            CommandName = name;
            return this;
        }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="CommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override AutocompleteCommandBuilder AddParameter(Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override AutocompleteCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new AutocompleteCommandInfo(this, module, commandService);
    }
}

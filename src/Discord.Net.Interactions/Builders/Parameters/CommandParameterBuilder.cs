using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="CommandParameterInfo"/>.
    /// </summary>
    public sealed class CommandParameterBuilder : ParameterBuilder<CommandParameterInfo, CommandParameterBuilder>
    {
        protected override CommandParameterBuilder Instance => this;

        internal CommandParameterBuilder(ICommandBuilder command) : base(command) { }

        /// <summary>
        ///     Initializes a new <see cref="CommandParameterInfo"/>.
        /// </summary>
        /// <param name="command">Parent command of this parameter.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="type">Type of this parameter.</param>
        public CommandParameterBuilder(ICommandBuilder command, string name, Type type) : base(command, name, type) { }

        internal override CommandParameterInfo Build(ICommandInfo command) =>
            new CommandParameterInfo(this, command);
    }
}

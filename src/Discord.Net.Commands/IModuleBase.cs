using Discord.Commands.Builders;

namespace Discord.Commands
{
    /// <summary>
    ///     Represents a generic module base.
    /// </summary>
    public interface IModuleBase
    {
        /// <summary>
        ///     Sets the context of this module base.
        /// </summary>
        /// <param name="context">The context to set.</param>
        void SetContext(ICommandContext context);

        /// <summary>
        ///     Executed before a command is run in this module base.
        /// </summary>
        /// <param name="command">The command thats about to run.</param>
        void BeforeExecute(CommandInfo command);

        /// <summary>
        ///     Executed after a command is ran in this module base.
        /// </summary>
        /// <param name="command">The command that ran.</param>
        void AfterExecute(CommandInfo command);

        /// <summary>
        ///     Executed when this module is building.
        /// </summary>
        /// <param name="commandService">The command service that is building this module.</param>
        /// <param name="builder">The builder constructing this module.</param>
        void OnModuleBuilding(CommandService commandService, ModuleBuilder builder);
    }
}

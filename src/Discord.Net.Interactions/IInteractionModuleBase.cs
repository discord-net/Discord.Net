namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a generic interaction module base.
    /// </summary>
    public interface IInteractionModuleBase
    {
        /// <summary>
        ///     Sets the context of this module.
        /// </summary>
        /// <param name="context"></param>
        void SetContext (IInteractionContext context);

        /// <summary>
        ///     Method body to be executed before executing an application command.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        void BeforeExecute (ICommandInfo command);

        /// <summary>
        ///     Method body to be executed after an application command execution.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        void AfterExecute (ICommandInfo command);

        /// <summary>
        ///     Method body to be executed before the derived module is built.
        /// </summary>
        /// <param name="commandService">Command Service instance that built this module.</param>
        /// <param name="module">Info class of this module.</param>
        void OnModuleBuilding (InteractionService commandService, ModuleInfo module);
    }
}

using System.Threading.Tasks;

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
        void SetContext(IInteractionContext context);

        /// <summary>
        ///     Method body to be executed asynchronously before executing an application command.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        Task BeforeExecuteAsync(ICommandInfo command);

        /// <summary>
        ///     Method body to be executed before executing an application command.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        void BeforeExecute(ICommandInfo command);

        /// <summary>
        ///     Method body to be executed asynchronously after an application command execution.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        Task AfterExecuteAsync(ICommandInfo command);

        /// <summary>
        ///     Method body to be executed after an application command execution.
        /// </summary>
        /// <param name="command">Command information related to the Discord Application Command.</param>
        void AfterExecute(ICommandInfo command);

        /// <summary>
        ///     Method body to be executed when <see cref="Builders.ModuleBuilder.Build(InteractionService, System.IServiceProvider, ModuleInfo)"/> is called.
        /// </summary>
        /// <param name="commandService">Command Service instance that built this module.</param>
        /// <param name="module">Info class of this module.</param>
        void OnModuleBuilding(InteractionService commandService, ModuleInfo module);

        /// <summary>
        ///     Method body to be executed after the automated module creation is completed and before <see cref="Builders.ModuleBuilder.Build(InteractionService, System.IServiceProvider, ModuleInfo)"/> is called.
        /// </summary>
        /// <param name="builder">Builder class of this module.</param>
        /// <param name="commandService">Command Service instance that is building this method.</param>
        void Construct(Builders.ModuleBuilder builder, InteractionService commandService);
    }
}

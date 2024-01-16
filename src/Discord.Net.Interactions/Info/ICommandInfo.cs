using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represent a command information object that can be executed.
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        ///     Gets the name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the name of the command handler method.
        /// </summary>
        string MethodName { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this command will be registered and executed as a standalone command, unaffected by the <see cref="GroupAttribute"/>s of
        ///     of the commands parents.
        /// </summary>
        bool IgnoreGroupNames { get; }

        /// <summary>
        ///     Gets whether this command supports wild card patterns.
        /// </summary>
        bool SupportsWildCards { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if this command is a top level command and none of its parents have a <see cref="GroupAttribute"/>.
        /// </summary>
        bool IsTopLevelCommand { get; }

        /// <summary>
        ///     Gets the module that the method belongs to.
        /// </summary>
        ModuleInfo Module { get; }

        /// <summary>
        ///     Gets the the underlying command service.
        /// </summary>
        InteractionService CommandService { get; }

        /// <summary>
        ///     Get the run mode this command gets executed with.
        /// </summary>
        RunMode RunMode { get; }

        /// <summary>
        ///     Gets a collection of the attributes of this command.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Gets a collection of the preconditions of this command.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Gets a collection of the parameters of this command.
        /// </summary>
        IReadOnlyCollection<IParameterInfo> Parameters { get; }

        bool TreatNameAsRegex { get; }

        /// <summary>
        ///     Executes the command with the provided context.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="services">Dependencies that will be used to create the module instance.</param>
        /// <returns>
        ///     A task representing the execution process. The task result contains the execution result.
        /// </returns>
        Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services);

        /// <summary>
        ///     Check if an execution context meets the command precondition requirements.
        /// </summary>
        Task<PreconditionResult> CheckPreconditionsAsync(IInteractionContext context, IServiceProvider services);
    }
}

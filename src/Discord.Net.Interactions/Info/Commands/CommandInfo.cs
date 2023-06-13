using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a cached method execution delegate.
    /// </summary>
    /// <param name="context">Execution context that will be injected into the module class.</param>
    /// <param name="args">Method arguments array.</param>
    /// <param name="serviceProvider">Service collection for initializing the module.</param>
    /// <param name="commandInfo">Command info class of the executed method.</param>
    /// <returns>
    ///     A task representing the execution operation.
    /// </returns>
    public delegate Task ExecuteCallback(IInteractionContext context, object[] args, IServiceProvider serviceProvider, ICommandInfo commandInfo);

    /// <summary>
    ///     The base information class for <see cref="InteractionService"/> commands.
    /// </summary>
    /// <typeparam name="TParameter">The type of <see cref="IParameterInfo"/> that is used by this command type.</typeparam>
    public abstract class CommandInfo<TParameter> : ICommandInfo where TParameter : class, IParameterInfo
    {
        private readonly ExecuteCallback _action;
        private readonly ILookup<string, PreconditionAttribute> _groupedPreconditions;

        internal IReadOnlyDictionary<string, TParameter> _parameterDictionary { get; }

        /// <inheritdoc/>
        public ModuleInfo Module { get; }

        /// <inheritdoc/>
        public InteractionService CommandService { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string MethodName { get; }

        /// <inheritdoc/>
        public virtual bool IgnoreGroupNames { get; }

        /// <inheritdoc/>
        public abstract bool SupportsWildCards { get; }

        /// <inheritdoc/>
        public bool IsTopLevelCommand { get; }

        /// <inheritdoc/>
        public RunMode RunMode { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        public abstract IReadOnlyList<TParameter> Parameters { get; }

        public bool TreatNameAsRegex { get; }

        internal CommandInfo(Builders.ICommandBuilder builder, ModuleInfo module, InteractionService commandService)
        {
            CommandService = commandService;
            Module = module;

            Name = builder.Name;
            MethodName = builder.MethodName;
            IgnoreGroupNames = builder.IgnoreGroupNames;
            IsTopLevelCommand = IgnoreGroupNames || CheckTopLevel(Module);
            RunMode = builder.RunMode != RunMode.Default ? builder.RunMode : commandService._runMode;
            Attributes = builder.Attributes.ToImmutableArray();
            Preconditions = builder.Preconditions.ToImmutableArray();
            TreatNameAsRegex = builder.TreatNameAsRegex && SupportsWildCards;

            _action = builder.Callback;
            _groupedPreconditions = builder.Preconditions.ToLookup(x => x.Group, x => x, StringComparer.Ordinal);
            _parameterDictionary = Parameters?.ToDictionary(x => x.Name, x => x).ToImmutableDictionary();
        }

        /// <inheritdoc/>
        public virtual async Task<IResult> ExecuteAsync(IInteractionContext context, IServiceProvider services)
        {
            switch (RunMode)
            {
                case RunMode.Sync:
                    return await ExecuteInternalAsync(context, services).ConfigureAwait(false);
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        await ExecuteInternalAsync(context, services).ConfigureAwait(false);
                    });
                    break;
                default:
                    throw new InvalidOperationException($"RunMode {RunMode} is not supported.");
            }

            return ExecuteResult.FromSuccess();
        }

        protected abstract Task<IResult> ParseArgumentsAsync(IInteractionContext context, IServiceProvider services);

        private async Task<IResult> ExecuteInternalAsync(IInteractionContext context, IServiceProvider services)
        {
            await CommandService._cmdLogger.DebugAsync($"Executing {GetLogString(context)}").ConfigureAwait(false);

            using var scope = services?.CreateScope();

            if (CommandService._autoServiceScopes)
                services = scope?.ServiceProvider ?? EmptyServiceProvider.Instance;

            try
            {
                var preconditionResult = await CheckPreconditionsAsync(context, services).ConfigureAwait(false);
                if (!preconditionResult.IsSuccess)
                    return await InvokeEventAndReturn(context, preconditionResult).ConfigureAwait(false);

                var argsResult = await ParseArgumentsAsync(context, services).ConfigureAwait(false);

                if (!argsResult.IsSuccess)
                    return await InvokeEventAndReturn(context, argsResult).ConfigureAwait(false);

                if (argsResult is not ParseResult parseResult)
                    return ExecuteResult.FromError(InteractionCommandError.BadArgs, "Complex command parsing failed for an unknown reason.");

                var args = parseResult.Args;

                var index = 0;
                foreach (var parameter in Parameters)
                {
                    var result = await parameter.CheckPreconditionsAsync(context, args[index++], services).ConfigureAwait(false);
                    if (!result.IsSuccess)
                        return await InvokeEventAndReturn(context, result).ConfigureAwait(false);
                }

                var task = _action(context, args, services, this);

                if (task is Task<IResult> resultTask)
                {
                    var result = await resultTask.ConfigureAwait(false);
                    await InvokeModuleEvent(context, result).ConfigureAwait(false);
                    if (result is RuntimeResult or ExecuteResult)
                        return result;
                }
                else
                {
                    await task.ConfigureAwait(false);
                    return await InvokeEventAndReturn(context, ExecuteResult.FromSuccess()).ConfigureAwait(false);
                }

                return await InvokeEventAndReturn(context, ExecuteResult.FromError(InteractionCommandError.Unsuccessful, "Command execution failed for an unknown reason")).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException)
                    ex = ex.InnerException;

                await Module.CommandService._cmdLogger.ErrorAsync(ex).ConfigureAwait(false);

                var result = ExecuteResult.FromError(ex);
                await InvokeModuleEvent(context, result).ConfigureAwait(false);

                if (Module.CommandService._throwOnError)
                {
                    if (ex == originalEx)
                        throw;
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                return result;
            }
            finally
            {
                await CommandService._cmdLogger.VerboseAsync($"Executed {GetLogString(context)}").ConfigureAwait(false);
            }
        }

        protected abstract Task InvokeModuleEvent(IInteractionContext context, IResult result);
        protected abstract string GetLogString(IInteractionContext context);

        /// <inheritdoc/>
        public async Task<PreconditionResult> CheckPreconditionsAsync(IInteractionContext context, IServiceProvider services)
        {
            async Task<PreconditionResult> CheckGroups(ILookup<string, PreconditionAttribute> preconditions, string type)
            {
                foreach (IGrouping<string, PreconditionAttribute> preconditionGroup in preconditions)
                {
                    if (preconditionGroup.Key == null)
                    {
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                        {
                            var result = await precondition.CheckRequirementsAsync(context, this, services).ConfigureAwait(false);
                            if (!result.IsSuccess)
                                return result;
                        }
                    }
                    else
                    {
                        var results = new List<PreconditionResult>();
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                            results.Add(await precondition.CheckRequirementsAsync(context, this, services).ConfigureAwait(false));

                        if (!results.Any(p => p.IsSuccess))
                            return PreconditionGroupResult.FromError($"{type} precondition group {preconditionGroup.Key} failed.", results);
                    }
                }
                return PreconditionGroupResult.FromSuccess();
            }

            var moduleResult = await CheckGroups(Module.GroupedPreconditions, "Module").ConfigureAwait(false);
            if (!moduleResult.IsSuccess)
                return moduleResult;

            var commandResult = await CheckGroups(_groupedPreconditions, "Command").ConfigureAwait(false);
            return !commandResult.IsSuccess ? commandResult : PreconditionResult.FromSuccess();
        }

        protected async Task<T> InvokeEventAndReturn<T>(IInteractionContext context, T result) where T : IResult
        {
            await InvokeModuleEvent(context, result).ConfigureAwait(false);
            return result;
        }

        private static bool CheckTopLevel(ModuleInfo parent)
        {
            var currentParent = parent;

            while (currentParent != null)
            {
                if (currentParent.IsSlashGroup)
                    return false;

                currentParent = currentParent.Parent;
            }
            return true;
        }

        // ICommandInfo

        /// <inheritdoc/>
        IReadOnlyCollection<IParameterInfo> ICommandInfo.Parameters => Parameters;

        /// <inheritdoc/>
        public override string ToString()
        {
            List<string> builder = new();

            var currentParent = Module;

            while (currentParent != null)
            {
                if (currentParent.IsSlashGroup)
                    builder.Add(currentParent.SlashGroupName);

                currentParent = currentParent.Parent;
            }
            builder.Reverse();
            builder.Add(Name);

            return string.Join(" ", builder);
        }
    }
}

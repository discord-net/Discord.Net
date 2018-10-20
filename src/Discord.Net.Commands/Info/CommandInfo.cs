using Discord.Commands.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Provides the information of a command.
    /// </summary>
    /// <remarks>
    ///     This object contains the information of a command. This can include the module of the command, various
    ///     descriptions regarding the command, and its <see cref="RunMode"/>.
    /// </remarks>
    [DebuggerDisplay("{Name,nq}")]
    public class CommandInfo
    {
        private static readonly System.Reflection.MethodInfo _convertParamsMethod = typeof(CommandInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<IEnumerable<object>, object>>();

        private readonly CommandService _commandService;
        private readonly Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> _action;

        /// <summary>
        ///     Gets the module that the command belongs in.
        /// </summary>
        public ModuleInfo Module { get; }
        /// <summary>
        ///     Gets the name of the command. If none is set, the first alias is used.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the summary of the command.
        /// </summary>
        /// <remarks>
        ///     This field returns the summary of the command. <see cref="Summary"/> and <see cref="Remarks"/> can be
        ///     useful in help commands and various implementation that fetches details of the command for the user.
        /// </remarks>
        public string Summary { get; }
        /// <summary>
        ///     Gets the remarks of the command.
        /// </summary>
        /// <remarks>
        ///     This field returns the summary of the command. <see cref="Summary"/> and <see cref="Remarks"/> can be
        ///     useful in help commands and various implementation that fetches details of the command for the user.
        /// </remarks>
        public string Remarks { get; }
        /// <summary>
        ///     Gets the priority of the command. This is used when there are multiple overloads of the command.
        /// </summary>
        public int Priority { get; }
        /// <summary>
        ///     Indicates whether the command accepts a <see langword="params"/> <see cref="Type"/>[] for its
        ///     parameter.
        /// </summary>
        public bool HasVarArgs { get; }
        /// <summary>
        ///     Indicates whether extra arguments should be ignored for this command.
        /// </summary>
        public bool IgnoreExtraArgs { get; }
        /// <summary>
        ///     Gets the <see cref="RunMode" /> that is being used for the command.
        /// </summary>
        public RunMode RunMode { get; }

        /// <summary>
        ///     Gets a list of aliases defined by the <see cref="AliasAttribute" /> of the command.
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }
        /// <summary>
        ///     Gets a list of information about the parameters of the command.
        /// </summary>
        public IReadOnlyList<ParameterInfo> Parameters { get; }
        /// <summary>
        ///     Gets a list of preconditions defined by the <see cref="PreconditionAttribute" /> of the command.
        /// </summary>
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }
        /// <summary>
        ///     Gets a list of attributes of the command.
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        internal CommandInfo(CommandBuilder builder, ModuleInfo module, CommandService service)
        {
            Module = module;

            Name = builder.Name;
            Summary = builder.Summary;
            Remarks = builder.Remarks;

            RunMode = (builder.RunMode == RunMode.Default ? service._defaultRunMode : builder.RunMode);
            Priority = builder.Priority;

            Aliases = module.Aliases
                .Permutate(builder.Aliases, (first, second) =>
                {
                    if (first == "")
                        return second;
                    else if (second == "")
                        return first;
                    else
                        return first + service._separatorChar + second;
                })
                .Select(x => service._caseSensitive ? x : x.ToLowerInvariant())
                .ToImmutableArray();

            Preconditions = builder.Preconditions.ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();

            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            HasVarArgs = builder.Parameters.Count > 0 && builder.Parameters[builder.Parameters.Count - 1].IsMultiple;
            IgnoreExtraArgs = builder.IgnoreExtraArgs;

            _action = builder.Callback;
            _commandService = service;
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IServiceProvider services = null)
        {
            services = services ?? EmptyServiceProvider.Instance;

            async Task<PreconditionResult> CheckGroups(IEnumerable<PreconditionAttribute> preconditions, string type)
            {
                foreach (IGrouping<string, PreconditionAttribute> preconditionGroup in preconditions.GroupBy(p => p.Group, StringComparer.Ordinal))
                {
                    if (preconditionGroup.Key == null)
                    {
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                        {
                            var result = await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false);
                            if (!result.IsSuccess)
                                return result;
                        }
                    }
                    else
                    {
                        var results = new List<PreconditionResult>();
                        foreach (PreconditionAttribute precondition in preconditionGroup)
                            results.Add(await precondition.CheckPermissionsAsync(context, this, services).ConfigureAwait(false));

                        if (!results.Any(p => p.IsSuccess))
                            return PreconditionGroupResult.FromError($"{type} precondition group {preconditionGroup.Key} failed.", results);
                    }
                }
                return PreconditionGroupResult.FromSuccess();
            }

            var moduleResult = await CheckGroups(Module.Preconditions, "Module").ConfigureAwait(false);
            if (!moduleResult.IsSuccess)
                return moduleResult;

            var commandResult = await CheckGroups(Preconditions, "Command").ConfigureAwait(false);
            if (!commandResult.IsSuccess)
                return commandResult;

            return PreconditionResult.FromSuccess();
        }

        public async Task<ParseResult> ParseAsync(ICommandContext context, int startIndex, SearchResult searchResult, PreconditionResult preconditionResult = null, IServiceProvider services = null)
        {
            services = services ?? EmptyServiceProvider.Instance;

            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);
            if (preconditionResult != null && !preconditionResult.IsSuccess)
                return ParseResult.FromError(preconditionResult);

            string input = searchResult.Text.Substring(startIndex);

            return await CommandParser.ParseArgsAsync(this, context, _commandService._ignoreExtraArgs, services, input, 0, _commandService._quotationMarkAliasMap).ConfigureAwait(false);
        }
        
        public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
        {
            if (!parseResult.IsSuccess)
                return Task.FromResult((IResult)ExecuteResult.FromError(parseResult));

            var argList = new object[parseResult.ArgValues.Count];
            for (int i = 0; i < parseResult.ArgValues.Count; i++)
            {
                if (!parseResult.ArgValues[i].IsSuccess)
                    return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ArgValues[i]));
                argList[i] = parseResult.ArgValues[i].Values.First().Value;
            }

            var paramList = new object[parseResult.ParamValues.Count];
            for (int i = 0; i < parseResult.ParamValues.Count; i++)
            {
                if (!parseResult.ParamValues[i].IsSuccess)
                    return Task.FromResult((IResult)ExecuteResult.FromError(parseResult.ParamValues[i]));
                paramList[i] = parseResult.ParamValues[i].Values.First().Value;
            }

            return ExecuteAsync(context, argList, paramList, services);
        }
        public async Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
        {
            services = services ?? EmptyServiceProvider.Instance;

            try
            {
                object[] args = GenerateArgs(argList, paramList);

                for (int position = 0; position < Parameters.Count; position++)
                {
                    var parameter = Parameters[position];
                    object argument = args[position];
                    var result = await parameter.CheckPreconditionsAsync(context, argument, services).ConfigureAwait(false);
                    if (!result.IsSuccess)
                        return ExecuteResult.FromError(result);
                }

                switch (RunMode)
                {
                    case RunMode.Sync: //Always sync
                        return await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                    case RunMode.Async: //Always async
                        var t2 = Task.Run(async () =>
                        {
                            await ExecuteInternalAsync(context, args, services).ConfigureAwait(false);
                        });
                        break;
                }
                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private async Task<IResult> ExecuteInternalAsync(ICommandContext context, object[] args, IServiceProvider services)
        {
            await Module.Service._cmdLogger.DebugAsync($"Executing {GetLogText(context)}").ConfigureAwait(false);
            try
            {
                var task = _action(context, args, services, this);
                if (task is Task<IResult> resultTask)
                {
                    var result = await resultTask.ConfigureAwait(false);
                    await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                    if (result is RuntimeResult execResult)
                        return execResult;
                }
                else if (task is Task<ExecuteResult> execTask)
                {
                    var result = await execTask.ConfigureAwait(false);
                    await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                    return result;
                }
                else
                {
                    await task.ConfigureAwait(false);
                    var result = ExecuteResult.FromSuccess();
                    await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);
                }

                var executeResult = ExecuteResult.FromSuccess();
                return executeResult;
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException) //Happens with void-returning commands
                    ex = ex.InnerException;

                var wrappedEx = new CommandException(this, context, ex);
                await Module.Service._cmdLogger.ErrorAsync(wrappedEx).ConfigureAwait(false);

                var result = ExecuteResult.FromError(ex);
                await Module.Service._commandExecutedEvent.InvokeAsync(this, context, result).ConfigureAwait(false);

                if (Module.Service._throwOnError)
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
                await Module.Service._cmdLogger.VerboseAsync($"Executed {GetLogText(context)}").ConfigureAwait(false);
            }
        }

        private object[] GenerateArgs(IEnumerable<object> argList, IEnumerable<object> paramsList)
        {
            int argCount = Parameters.Count;
            var array = new object[Parameters.Count];
            if (HasVarArgs)
                argCount--;

            int i = 0;
            foreach (object arg in argList)
            {
                if (i == argCount)
                    throw new InvalidOperationException("Command was invoked with too many parameters.");
                array[i++] = arg;
            }
            if (i < argCount)
                throw new InvalidOperationException("Command was invoked with too few parameters.");

            if (HasVarArgs)
            {
                var func = _arrayConverters.GetOrAdd(Parameters[Parameters.Count - 1].Type, t =>
                {
                    var method = _convertParamsMethod.MakeGenericMethod(t);
                    return (Func<IEnumerable<object>, object>)method.CreateDelegate(typeof(Func<IEnumerable<object>, object>));
                });
                array[i] = func(paramsList);
            }

            return array;
        }

        private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList)
            => paramsList.Cast<T>().ToArray();

        internal string GetLogText(ICommandContext context)
        {
            if (context.Guild != null)
                return $"\"{Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"\"{Name}\" for {context.User} in {context.Channel}";
        }
    }
}

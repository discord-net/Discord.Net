using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

using Discord.Commands.Builders;
using System.Runtime.ExceptionServices;

namespace Discord.Commands
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class OverloadInfo
    {
        private static readonly MethodInfo _convertParamsMethod = typeof(OverloadInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<IEnumerable<object>, object>>();

        private readonly Func<ICommandContext, object[], IServiceProvider, OverloadInfo, Task> _action;

        public CommandInfo Command { get; }
        public int Priority { get; }
        public bool HasVarArgs { get; }
        public RunMode RunMode { get; }

        public IReadOnlyList<ParameterInfo> Parameters { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        internal OverloadInfo(OverloadBuilder builder, CommandInfo command, CommandService service)
        {
            Command = command;

            RunMode = (builder.RunMode == RunMode.Default ? service._defaultRunMode : builder.RunMode);
            Priority = builder.Priority;

            Preconditions = builder.Preconditions.ToImmutableArray();

            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            HasVarArgs = builder.Parameters.Count > 0 ? builder.Parameters[builder.Parameters.Count - 1].IsMultiple : false;

            _action = builder.Callback;
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IServiceProvider services = null)
        {
            if (services == null)
                services = EmptyServiceProvider.Instance;

            foreach (PreconditionAttribute precondition in Command.Module.Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            foreach (PreconditionAttribute precondition in Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        public async Task<ParseResult> ParseAsync(ICommandContext context, IServiceProvider services, SearchResult searchResult, PreconditionResult? preconditionResult = null)
        {
            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);
            if (preconditionResult != null && !preconditionResult.Value.IsSuccess)
                return ParseResult.FromError(preconditionResult.Value);

            string input = searchResult.Text;
            var matchingAliases = Command.Aliases.Where(alias => input.StartsWith(alias));

            string matchingAlias = "";
            foreach (string alias in matchingAliases)
            {
                if (alias.Length > matchingAlias.Length)
                    matchingAlias = alias;
            }

            input = input.Substring(matchingAlias.Length);

            return await CommandParser.ParseArgs(this, context, services, input, 0).ConfigureAwait(false);
        }

        public Task<ExecuteResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
        {
            if (!parseResult.IsSuccess)
                return Task.FromResult(ExecuteResult.FromError(parseResult));

            var argList = new object[parseResult.ArgValues.Count];
            for (int i = 0; i < parseResult.ArgValues.Count; i++)
            {
                if (!parseResult.ArgValues[i].IsSuccess)
                    return Task.FromResult(ExecuteResult.FromError(parseResult.ArgValues[i]));
                argList[i] = parseResult.ArgValues[i].Values.First().Value;
            }

            var paramList = new object[parseResult.ParamValues.Count];
            for (int i = 0; i < parseResult.ParamValues.Count; i++)
            {
                if (!parseResult.ParamValues[i].IsSuccess)
                    return Task.FromResult(ExecuteResult.FromError(parseResult.ParamValues[i]));
                paramList[i] = parseResult.ParamValues[i].Values.First().Value;
            }

            return ExecuteAsync(context, argList, paramList, services);
        }
        public async Task<ExecuteResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
        {
            if (services == null)
                services = EmptyServiceProvider.Instance;

            try
            {
                var args = GenerateArgs(argList, paramList);
                switch (RunMode)
                {
                    case RunMode.Sync: //Always sync
                        await ExecuteAsyncInternal(context, args, services).ConfigureAwait(false);
                        break;
                    case RunMode.Async: //Always async
                        var t2 = Task.Run(() => ExecuteAsyncInternal(context, args, services));
                        break;
                }
                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private async Task ExecuteAsyncInternal(ICommandContext context, object[] args, IServiceProvider map)
        {
            await Command.Module.Service._cmdLogger.DebugAsync($"Executing {GetLogText(context)}").ConfigureAwait(false);
            try
            {
                await _action(context, args, map, this).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var originalEx = ex;
                while (ex is TargetInvocationException) //Happens with void-returning commands
                    ex = ex.InnerException;

                var wrappedEx = new CommandException(this, context, ex);
                await Command.Module.Service._cmdLogger.ErrorAsync(wrappedEx).ConfigureAwait(false);
                if (Command.Module.Service._throwOnError)
                {
                    if (ex == originalEx)
                        throw;
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            await Command.Module.Service._cmdLogger.VerboseAsync($"Executed {GetLogText(context)}").ConfigureAwait(false);
        }

        internal string GetLogText(ICommandContext context)
        {
            if (context.Guild != null)
                return $"\"{Command.Name}\" for {context.User} in {context.Guild}/{context.Channel}";
            else
                return $"\"{Command.Name}\" for {context.User} in {context.Channel}";
        }

        private object[] GenerateArgs(IEnumerable<object> argList, IEnumerable<object> paramsList)
        {
            int argCount = Parameters.Count;
            var array = new object[Parameters.Count];
            if (HasVarArgs)
                argCount--;

            int i = 0;
            foreach (var arg in argList)
            {
                if (i == argCount)
                    throw new InvalidOperationException("Command was invoked with too many parameters");
                array[i++] = arg;
            }
            if (i < argCount)
                throw new InvalidOperationException("Command was invoked with too few parameters");

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

        private string DebuggerDisplay => $"{Command.Name} ({Priority}, {RunMode})";

        private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList)
            => paramsList.Cast<T>().ToArray();
    }
}

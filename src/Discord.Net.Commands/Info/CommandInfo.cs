using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Reflection;

using Discord.Commands.Builders;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay("{Name,nq}")]
    public class CommandInfo
    {
        private static readonly System.Reflection.MethodInfo _convertParamsMethod = typeof(CommandInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<IEnumerable<object>, object>>();

        private readonly Func<ICommandContext, object[], IDependencyMap, Task> _action;

        public ModuleInfo Module { get; }
        public string Name { get; }
        public string Summary { get; }
        public string Remarks { get; }
        public int Priority { get; }
        public bool HasVarArgs { get; }
        public RunMode RunMode { get; }

        public IReadOnlyList<string> Aliases { get; }
        public IReadOnlyList<ParameterInfo> Parameters { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

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

            Parameters = builder.Parameters.Select(x => x.Build(this)).ToImmutableArray();
            HasVarArgs = builder.Parameters.Count > 0 ? builder.Parameters[builder.Parameters.Count - 1].IsMultiple : false;

            _action = builder.Callback;
        }

        public IEnumerable<TPrecondition> GetPrecondition<TPrecondition>() where TPrecondition : PreconditionAttribute =>
            Preconditions.Where(x => x.GetType() == typeof(TPrecondition)).Select(x => x as TPrecondition);

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IDependencyMap map = null)
        {
            if (map == null)
                map = DependencyMap.Empty;

            foreach (PreconditionAttribute precondition in Module.Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, map).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            foreach (PreconditionAttribute precondition in Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, map).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }
        
        public async Task<ParseResult> ParseAsync(ICommandContext context, int startIndex, SearchResult searchResult, PreconditionResult? preconditionResult = null)
        {
            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);
            if (preconditionResult != null && !preconditionResult.Value.IsSuccess)
                return ParseResult.FromError(preconditionResult.Value);
            
            string input = searchResult.Text.Substring(startIndex);
            return await CommandParser.ParseArgs(this, context, input, 0).ConfigureAwait(false);
        }

        public Task<ExecuteResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IDependencyMap map)
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

            return ExecuteAsync(context, argList, paramList, map);
        }
        public async Task<ExecuteResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IDependencyMap map)
        {
            if (map == null)
                map = DependencyMap.Empty;

            try
            {
                object[] args = GenerateArgs(argList, paramList);

                foreach (var parameter in Parameters)
                {
                    var result = await parameter.CheckPreconditionsAsync(context, args, map).ConfigureAwait(false);
                    if (!result.IsSuccess)
                        return ExecuteResult.FromError(result);
                }

                switch (RunMode)
                {
                    case RunMode.Sync: //Always sync
                        await _action(context, args, map).ConfigureAwait(false);
                        break;
                    case RunMode.Mixed: //Sync until first await statement
                        var t1 = _action(context, args, map);
                        break;
                    case RunMode.Async: //Always async
                        var t2 = Task.Run(() => _action(context, args, map));
                        break;
                }
                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
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

        private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList)
            => paramsList.Cast<T>().ToArray();
    }
}
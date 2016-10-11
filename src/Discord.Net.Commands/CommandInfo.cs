using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CommandInfo
    {
        private static readonly MethodInfo _convertParamsMethod = typeof(CommandInfo).GetTypeInfo().GetDeclaredMethod(nameof(ConvertParamsList));
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new ConcurrentDictionary<Type, Func<IEnumerable<object>, object>>();
        
        private readonly Func<CommandContext, object[], Task> _action;

        public MethodInfo Source { get; }
        public ModuleInfo Module { get; }
        public string Name { get; }
        public string Summary { get; }
        public string Remarks { get; }
        public string Text { get; }
        public int Priority { get; }
        public bool HasVarArgs { get; }
        public RunMode RunMode { get; }
        public IReadOnlyList<string> Aliases { get; }
        public IReadOnlyList<CommandParameter> Parameters { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        internal CommandInfo(MethodInfo source, ModuleInfo module, CommandAttribute attribute, string groupPrefix)
        {
            try
            {
                Source = source;
                Module = module;

                Name = source.Name;

                if (attribute.Text == null)
                    Text = groupPrefix;
                RunMode = attribute.RunMode;

                if (groupPrefix != "")
                    groupPrefix += " ";

                if (attribute.Text != null)
                    Text = groupPrefix + attribute.Text;

                var aliasesBuilder = ImmutableArray.CreateBuilder<string>();

                aliasesBuilder.Add(Text);

                var aliasesAttr = source.GetCustomAttribute<AliasAttribute>();
                if (aliasesAttr != null)
                    aliasesBuilder.AddRange(aliasesAttr.Aliases.Select(x => groupPrefix + x));

                Aliases = aliasesBuilder.ToImmutable();

                var nameAttr = source.GetCustomAttribute<NameAttribute>();
                if (nameAttr != null)
                    Name = nameAttr.Text;

                var summary = source.GetCustomAttribute<SummaryAttribute>();
                if (summary != null)
                    Summary = summary.Text;

                var remarksAttr = source.GetCustomAttribute<RemarksAttribute>();
                if (remarksAttr != null)
                    Remarks = remarksAttr.Text;

                var priorityAttr = source.GetCustomAttribute<PriorityAttribute>();
                Priority = priorityAttr?.Priority ?? 0;

                Parameters = BuildParameters(source);
                HasVarArgs = Parameters.Count > 0 ? Parameters[Parameters.Count - 1].IsMultiple : false;
                Preconditions = BuildPreconditions(source);
                _action = BuildAction(source);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to build command {source.DeclaringType.FullName}.{source.Name}", ex);
            }
        }

        public async Task<PreconditionResult> CheckPreconditions(CommandContext context, IDependencyMap map = null)
        {
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

        public async Task<ParseResult> Parse(CommandContext context, SearchResult searchResult, PreconditionResult? preconditionResult = null)
        {
            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);
            if (preconditionResult != null && !preconditionResult.Value.IsSuccess)
                return ParseResult.FromError(preconditionResult.Value);

            string input = searchResult.Text;
            var matchingAliases = Aliases.Where(alias => input.StartsWith(alias));
            
            string matchingAlias = "";
            foreach (string alias in matchingAliases)
            {
                if (alias.Length > matchingAlias.Length)
                    matchingAlias = alias;
            }
            
            input = input.Substring(matchingAlias.Length);

            return await CommandParser.ParseArgs(this, context, input, 0).ConfigureAwait(false);
        }
        public Task<ExecuteResult> Execute(CommandContext context, ParseResult parseResult)
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

            return Execute(context, argList, paramList);
        }
        public async Task<ExecuteResult> Execute(CommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList)
        {
            try
            {
                var args = GenerateArgs(argList, paramList);
                switch (RunMode)
                {
                    case RunMode.Sync:
                        await _action(context, args).ConfigureAwait(false);
                        break;
                    case RunMode.Async:
                        var t1 = _action(context, args);
                        break;
                    case RunMode.FireAndForget:
                        var t2 = Task.Run(() => _action(context, args));
                        break;
                }
                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex);
            }
        }

        private IReadOnlyList<PreconditionAttribute> BuildPreconditions(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<PreconditionAttribute>().ToImmutableArray();
        }

        private IReadOnlyList<CommandParameter> BuildParameters(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            var paramBuilder = ImmutableArray.CreateBuilder<CommandParameter>(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var type = parameter.ParameterType;

                //Detect 'params'
                bool isMultiple = parameter.GetCustomAttribute<ParamArrayAttribute>() != null;
                if (isMultiple)
                    type = type.GetElementType();

                var reader = Module.Service.GetTypeReader(type);
                var typeInfo = type.GetTypeInfo();

                //Detect enums
                if (reader == null && typeInfo.IsEnum)
                {
                    reader = EnumTypeReader.GetReader(type);
                    Module.Service.AddTypeReader(type, reader);
                }

                if (reader == null)
                    throw new InvalidOperationException($"{type.FullName} is not supported as a command parameter, are you missing a TypeReader?");

                bool isRemainder = parameter.GetCustomAttribute<RemainderAttribute>() != null;
                if (isRemainder && i != parameters.Length - 1)
                    throw new InvalidOperationException("Remainder parameters must be the last parameter in a command.");

                string name = parameter.Name;
                string summary = parameter.GetCustomAttribute<SummaryAttribute>()?.Text;
                bool isOptional = parameter.IsOptional;
                object defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                paramBuilder.Add(new CommandParameter(parameters[i], name, summary, type, reader, isOptional, isRemainder, isMultiple, defaultValue));
            }
            return paramBuilder.ToImmutable();
        }
        private Func<CommandContext, object[], Task> BuildAction(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task))
                throw new InvalidOperationException("Commands must return a non-generic Task.");

            return (context, args) =>
            {
                var instance = Module.CreateInstance();
                instance.Context = context;
                try
                {
                    return methodInfo.Invoke(instance, args) as Task ?? Task.CompletedTask;
                }
                finally
                {
                    (instance as IDisposable)?.Dispose();
                }
            };
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
                var func = _arrayConverters.GetOrAdd(Parameters[Parameters.Count - 1].ElementType, t =>
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

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Module.Name}.{Name} ({Text})";
    }
}

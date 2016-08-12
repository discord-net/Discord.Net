using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Command
    {
        private readonly object _instance;
        private readonly Func<IMessage, IReadOnlyList<object>, Task> _action;

        public MethodInfo Source { get; }
        public string Name { get; }
        public string Description { get; }
        public string Summary { get; }
        public string Text { get; }
        public Module Module { get; }
        public IReadOnlyList<CommandParameter> Parameters { get; }
        public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

        internal Command(MethodInfo source, Module module, object instance, CommandAttribute attribute, string groupPrefix)
        {
            Source = source;
            Module = module;
            _instance = instance;

            Name = source.Name;
            Text = groupPrefix + attribute.Text;

            var description = source.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
                Description = description.Text;

            var summary = source.GetCustomAttribute<SummaryAttribute>();
            if (summary != null)
                Summary = summary.Text;

            Parameters = BuildParameters(source);
            Preconditions = BuildPreconditions(source);
            _action = BuildAction(source);
        }

        public async Task<PreconditionResult> CheckPreconditions(IMessage context)
        {
            foreach (PreconditionAttribute precondition in Module.Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, Module.Instance).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            foreach (PreconditionAttribute precondition in Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, Module.Instance).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        public async Task<ParseResult> Parse(IMessage msg, SearchResult searchResult, PreconditionResult? preconditionResult = null)
        {
            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);
            if (preconditionResult != null && !preconditionResult.Value.IsSuccess)
                return ParseResult.FromError(preconditionResult.Value);

            return await CommandParser.ParseArgs(this, msg, searchResult.Text.Substring(Text.Length), 0).ConfigureAwait(false);
        }
        public async Task<ExecuteResult> Execute(IMessage msg, ParseResult parseResult)
        {
            if (!parseResult.IsSuccess)
                return ExecuteResult.FromError(parseResult);

            try
            {
                await _action.Invoke(msg, parseResult.Values);//Note: This code may need context
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
            var paramBuilder = ImmutableArray.CreateBuilder<CommandParameter>(parameters.Length - 1);
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var type = parameter.ParameterType;

                if (i == 0)
                {
                    if (type != typeof(IMessage))
                        throw new InvalidOperationException("The first parameter of a command must be IMessage.");
                    else
                        continue;
                }

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
                string summary = parameter.GetCustomAttribute<DescriptionAttribute>()?.Text;
                bool isOptional = parameter.IsOptional;
                object defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                paramBuilder.Add(new CommandParameter(parameters[i], name, summary, type, reader, isOptional, isRemainder, isMultiple, defaultValue));
            }
            return paramBuilder.ToImmutable();
        }
        private Func<IMessage, IReadOnlyList<object>, Task> BuildAction(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task))
                throw new InvalidOperationException("Commands must return a non-generic Task.");

            return (msg, args) =>
            {
                object[] newArgs = new object[args.Count + 1];
                newArgs[0] = msg;
                for (int i = 0; i < args.Count; i++)
                    newArgs[i + 1] = args[i];
                var result = methodInfo.Invoke(_instance, newArgs);
                return result as Task ?? Task.CompletedTask;
            };
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Module.Name}.{Name} ({Text})";
    }
}

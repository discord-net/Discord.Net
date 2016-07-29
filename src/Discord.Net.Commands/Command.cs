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

        public string Name { get; }
        public string Description { get; }
        public string Text { get; }
        public Module Module { get; }
        public IReadOnlyList<CommandParameter> Parameters { get; }
        
        internal Command(Module module, object instance, CommandAttribute attribute, MethodInfo methodInfo, string groupPrefix)
        {
            Module = module;
            _instance = instance;

            Name = methodInfo.Name;
            Text = groupPrefix + attribute.Text;

            var description = methodInfo.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
                Description = description.Text;

            Parameters = BuildParameters(methodInfo);
            _action = BuildAction(methodInfo);
        }

        public async Task<ParseResult> Parse(IMessage msg, SearchResult searchResult)
        {
            if (!searchResult.IsSuccess)
                return ParseResult.FromError(searchResult);

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

                var typeInfo = type.GetTypeInfo();
                var reader = Module.Service.GetTypeReader(type);

                if (reader == null && typeInfo.IsEnum)
                {
                    reader = EnumTypeReader.GetReader(type);
                    Module.Service.AddTypeReader(type, reader);
                }

                if (reader == null)
                    throw new InvalidOperationException($"{type.FullName} is not supported as a command parameter, are you missing a TypeReader?");

                bool isUnparsed = parameter.GetCustomAttribute<UnparsedAttribute>() != null;
                if (isUnparsed && i != parameters.Length - 1)
                    throw new InvalidOperationException("Unparsed parameters must be the last parameter in a command.");

                string name = parameter.Name;
                string description = typeInfo.GetCustomAttribute<DescriptionAttribute>()?.Text;
                bool isOptional = parameter.IsOptional;
                object defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                paramBuilder.Add(new CommandParameter(name, description, reader, isOptional, isUnparsed, defaultValue));
            }
            return paramBuilder.ToImmutable();
        }
        private Func<IMessage, IReadOnlyList<object>, Task> BuildAction(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task))
                throw new InvalidOperationException("Commands must return a non-generic Task.");

            //TODO: Temporary reflection hack. Lets build an actual expression tree here.
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

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
        public string Synopsis { get; }
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

            var synopsis = methodInfo.GetCustomAttribute<SynopsisAttribute>();
            if (synopsis != null)
                Synopsis = synopsis.Text;

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
                Type underlyingType = null;

                if (i == 0)
                {
                    if (type != typeof(IMessage))
                        throw new InvalidOperationException("The first parameter of a command must be IMessage.");
                    else
                        continue;
                }

                var reader = Module.Service.GetTypeReader(type);

                // TODO: is there a better way of detecting 'params'?
                bool isParams = type.IsArray && i == parameters.Length - 1;
                if (isParams)
                {
                    underlyingType = type.GetElementType();
                    reader = Module.Service.GetTypeReader(underlyingType);
                }
                else
                {
                    underlyingType = type;
                }

                var underlyingTypeInfo = underlyingType.GetTypeInfo();
                var typeInfo = type.GetTypeInfo();

                if (reader == null && underlyingTypeInfo.IsEnum)
                {
                    reader = EnumTypeReader.GetReader(underlyingType);
                    Module.Service.AddTypeReader(type, reader);
                }

                if (reader == null)
                    throw new InvalidOperationException($"{type.FullName} is not supported as a command parameter, are you missing a TypeReader?");

                bool isRemainder = parameter.GetCustomAttribute<RemainderAttribute>() != null;
                if (isRemainder && i != parameters.Length - 1)
                    throw new InvalidOperationException("Remainder parameters must be the last parameter in a command.");

                string name = parameter.Name;
                string description = typeInfo.GetCustomAttribute<DescriptionAttribute>()?.Text;
                bool isOptional = parameter.IsOptional;
                object defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                paramBuilder.Add(new CommandParameter(name, description, type, underlyingType, reader, isOptional, isRemainder, isParams, defaultValue));
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

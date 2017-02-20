using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Discord.Commands.Builders;
using System.Reflection;

namespace Discord.Commands
{
    public class ParameterInfo
    {

        private static MethodInfo _checkPermissionsMethod = typeof(ParameterPreconditionAttribute)
            .GetTypeInfo().GetDeclaredMethod("CheckPermissions");

        private readonly TypeReader _reader;
        private readonly MethodInfo _specializedCheckPermissionsMethod;

        public CommandInfo Command { get; }
        public string Name { get; }
        public string Summary { get; }
        public bool IsOptional { get; }
        public bool IsRemainder { get; }
        public bool IsMultiple { get; }
        public Type Type { get; }
        public object DefaultValue { get; }

        public IReadOnlyList<ParameterPreconditionAttribute> Preconditions { get; }

        internal ParameterInfo(ParameterBuilder builder, CommandInfo command, CommandService service)
        {
            Command = command;

            Name = builder.Name;
            Summary = builder.Summary;
            IsOptional = builder.IsOptional;
            IsRemainder = builder.IsRemainder;
            IsMultiple = builder.IsMultiple;

            Type = builder.ParameterType;
            DefaultValue = builder.DefaultValue;

            Preconditions = builder.Preconditions.ToImmutableArray();

            _reader = builder.TypeReader;

            _specializedCheckPermissionsMethod = _checkPermissionsMethod.MakeGenericMethod(Type);
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, object arg, IDependencyMap map = null)
        {
            if (map == null)
                map = DependencyMap.Empty;

            foreach (var precondition in Preconditions)
            {
                object[] parameters = new []{ context, this, arg, map };
                var resultTask = (_specializedCheckPermissionsMethod.Invoke(precondition, parameters) as Task<PreconditionResult>);
                var result = await resultTask.ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        public async Task<TypeReaderResult> Parse(ICommandContext context, string input)
        {
            return await _reader.Read(context, input).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}
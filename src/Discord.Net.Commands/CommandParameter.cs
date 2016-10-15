using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands.Attributes;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CommandParameter
    {
        private readonly TypeReader _reader;

        public ParameterInfo Source { get; }
        public string Name { get; }
        public string Summary { get; }
        public bool IsOptional { get; }
        public bool IsRemainder { get; }
        public bool IsMultiple { get; }
        public Type ElementType { get; }
        public object DefaultValue { get; }
        public IReadOnlyList<ParameterPreconditionAttribute> Preconditions { get; }

        public CommandParameter(ParameterInfo source, string name, string summary, Type type, TypeReader reader, bool isOptional, bool isRemainder, bool isMultiple, object defaultValue)
        {
            Source = source;
            Name = name;
            Summary = summary;
            ElementType = type;
            _reader = reader;
            IsOptional = isOptional;
            IsRemainder = isRemainder;
            IsMultiple = isMultiple;
            DefaultValue = defaultValue;
            Preconditions = BuildPreconditions(source);
        }

        public async Task<TypeReaderResult> Parse(CommandContext context, string input)
        {
            return await _reader.Read(context, input).ConfigureAwait(false);
        }

        public async Task<PreconditionResult> CheckPreconditions(CommandContext context, IDependencyMap map = null)
        {
            foreach (ParameterPreconditionAttribute precondition in Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, map).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        private IReadOnlyList<ParameterPreconditionAttribute> BuildPreconditions(ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttributes<ParameterPreconditionAttribute>().ToImmutableArray();
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}

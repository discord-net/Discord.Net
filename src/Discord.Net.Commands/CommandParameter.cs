using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

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
        }

        public async Task<TypeReaderResult> Parse(IUserMessage context, string input)
        {
            return await _reader.Read(context, input).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}

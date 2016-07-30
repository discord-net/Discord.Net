using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Commands
{
    //TODO: Add support for Multiple
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CommandParameter
    {
        private readonly TypeReader _reader;

        public string Name { get; }
        public string Description { get; }
        public bool IsOptional { get; }
        public bool IsRemainder { get; }
        public Type Type { get; }
        internal object DefaultValue { get; }

        public CommandParameter(string name, string description, Type type, TypeReader reader, bool isOptional, bool isRemainder, object defaultValue)
        {
            Name = name;
            Description = description;
            Type = type;
            _reader = reader;
            IsOptional = isOptional;
            IsRemainder = isRemainder;
            DefaultValue = defaultValue;
        }

        public async Task<TypeReaderResult> Parse(IMessage context, string input)
        {
            return await _reader.Read(context, input).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}

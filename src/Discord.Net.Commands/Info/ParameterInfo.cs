using System;
using System.Linq;
using System.Threading.Tasks;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    public class ParameterInfo
    {
        private readonly TypeReader _reader;

        internal ParameterInfo(ParameterBuilder builder, CommandInfo command, CommandService service)
        {
            Command = command;

            Name = builder.Name;
            Summary = builder.Summary;
            IsOptional = builder.Optional;
            IsRemainder = builder.Remainder;
            IsMultiple = builder.Multiple;

            ParameterType = builder.ParameterType;
            DefaultValue = builder.DefaultValue;

            _reader = builder.TypeReader;
        }

        public CommandInfo Command { get; }
        public string Name { get; }
        public string Summary { get; }
        public bool IsOptional { get; }
        public bool IsRemainder { get; }
        public bool IsMultiple { get; }
        public Type ParameterType { get; }
        public object DefaultValue { get; }

        public async Task<TypeReaderResult> Parse(CommandContext context, string input)
        {
            return await _reader.Read(context, input).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Discord.Commands.Builders;

namespace Discord.Commands
{
    public class ParameterInfo
    {
        private readonly TypeReader _reader;

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
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, object[] args, IDependencyMap map = null)
        {
            if (map == null)
                map = DependencyMap.Empty;

            int position = 0;
            for(position = 0; position < Command.Parameters.Count; position++)
                if (Command.Parameters[position] == this)
                    break;

            foreach (var precondition in Preconditions)
            {
                var result = await precondition.CheckPermissions(context, this, args[position], map).ConfigureAwait(false);
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
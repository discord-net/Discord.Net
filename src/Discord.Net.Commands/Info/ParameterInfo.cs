using Discord.Commands.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Provides the information of a parameter.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class ParameterInfo
    {
        private readonly TypeReader _reader;

        /// <summary>
        ///     Gets the command that associates with this parameter.
        /// </summary>
        public CommandInfo Command { get; }
        /// <summary>
        ///     Gets the name of this parameter.
        /// </summary>
        public string Name { get; }
        /// <summary>
        ///     Gets the summary of this parameter.
        /// </summary>
        public string Summary { get; }
        /// <summary>
        ///     Gets a value that indicates whether this parameter is optional or not.
        /// </summary>
        public bool IsOptional { get; }
        /// <summary>
        ///     Gets a value that indicates whether this parameter is a remainder parameter or not.
        /// </summary>
        public bool IsRemainder { get; }
        public bool IsMultiple { get; }
        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        ///     Gets the default value for this optional parameter if applicable.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        ///     Gets a read-only list of precondition that apply to this parameter.
        /// </summary>
        public IReadOnlyList<ParameterPreconditionAttribute> Preconditions { get; }
        /// <summary>
        ///     Gets a read-only list of attributes that apply to this parameter.
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

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
            Attributes = builder.Attributes.ToImmutableArray();

            _reader = builder.TypeReader;
        }

        public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, object arg, IServiceProvider services = null)
        {
            services = services ?? EmptyServiceProvider.Instance;

            foreach (var precondition in Preconditions)
            {
                var result = await precondition.CheckPermissionsAsync(context, this, arg, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        public async Task<TypeReaderResult> ParseAsync(ICommandContext context, string input, IServiceProvider services = null)
        {
            services = services ?? EmptyServiceProvider.Instance;
            return await _reader.ReadAsync(context, input, services).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
    }
}

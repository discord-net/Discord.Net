using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base parameter info class for <see cref="InteractionService"/> commands.
    /// </summary>
    public class CommandParameterInfo : IParameterInfo
    {
        /// <inheritdoc/>
        public ICommandInfo Command { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type ParameterType { get; }

        /// <inheritdoc/>
        public bool IsRequired { get; }

        /// <inheritdoc/>
        public bool IsParameterArray { get; }

        /// <inheritdoc/>
        public object DefaultValue { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        internal CommandParameterInfo(Builders.IParameterBuilder builder, ICommandInfo command)
        {
            Command = command;
            Name = builder.Name;
            ParameterType = builder.ParameterType;
            IsRequired = builder.IsRequired;
            IsParameterArray = builder.IsParameterArray;
            DefaultValue = builder.DefaultValue;
            Attributes = builder.Attributes.ToImmutableArray();
            Preconditions = builder.Preconditions.ToImmutableArray();
        }

        /// <inheritdoc/>
        public async Task<PreconditionResult> CheckPreconditionsAsync(IInteractionContext context, object value, IServiceProvider services)
        {
            foreach (var precondition in Preconditions)
            {
                var result = await precondition.CheckRequirementsAsync(context, this, value, services).ConfigureAwait(false);
                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }
    }
}

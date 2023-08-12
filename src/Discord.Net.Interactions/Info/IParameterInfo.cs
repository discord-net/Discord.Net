using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a <see cref="ICommandInfo"/> parameter.
    /// </summary>
    public interface IParameterInfo
    {
        /// <summary>
        ///     Gets the command that this parameter belongs to.
        /// </summary>
        ICommandInfo Command { get; }

        /// <summary>
        ///     Gets the name of this parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of this parameter.
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        ///     Gets whether this parameter is required.
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        ///     Gets whether this parameter is marked with a <see langword="params"/> keyword.
        /// </summary>
        bool IsParameterArray { get; }

        /// <summary>
        ///     Gets the default value of this parameter if the parameter is optional.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        ///     Gets a list of the attributes this parameter has.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Gets a list of the preconditions this parameter has.
        /// </summary>
        IReadOnlyCollection<ParameterPreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     Check if an execution context meets the parameter precondition requirements.
        /// </summary>
        Task<PreconditionResult> CheckPreconditionsAsync(IInteractionContext context, object value, IServiceProvider services);
    }
}

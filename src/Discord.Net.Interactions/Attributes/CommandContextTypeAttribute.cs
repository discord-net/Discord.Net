using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions;

/// <summary>
///     Specifies context types this command can be executed in.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandContextTypeAttribute : Attribute
{
    /// <summary>
    ///     Gets context types this command can be executed in.
    /// </summary>
    public IReadOnlyCollection<InteractionContextType> ContextTypes { get; }

    /// <summary>
    ///     Sets the <see cref="IApplicationCommandInfo.ContextTypes"/> property of an application command or module.
    /// </summary>
    /// <param name="contextTypes">Context types set for the command.</param>
    public CommandContextTypeAttribute(params InteractionContextType[] contextTypes)
    {
        ContextTypes = contextTypes?.Distinct().ToImmutableArray()
                       ?? throw new ArgumentNullException(nameof(contextTypes));

        if (ContextTypes.Count == 0)
            throw new ArgumentException("A command must have at least one supported context type.", nameof(contextTypes));
    }
}

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
    public IReadOnlyCollection<InteractionContextType> ContextTypes { get; }

    public CommandContextTypeAttribute(params InteractionContextType[] contextTypes)
    {
        ContextTypes = contextTypes?.Distinct().ToImmutableArray()
                       ?? throw new ArgumentNullException(nameof(contextTypes));
    }
}

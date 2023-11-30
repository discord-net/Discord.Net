using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Interactions;

/// <summary>
///     Specifies context types this command can be executed in.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ContextTypeAttribute : Attribute
{
    public IReadOnlyCollection<ApplicationCommandContextType> ContextTypes { get; }

    public ContextTypeAttribute(params ApplicationCommandContextType[] contextTypes)
    {
        ContextTypes = contextTypes?.Distinct().ToImmutableArray()
                       ?? throw new ArgumentNullException(nameof(contextTypes));
    }
}

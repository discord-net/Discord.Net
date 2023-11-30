using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions;

/// <summary>
///     Specifies install method for the command.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class IntegrationTypeAttribute : Attribute
{
    public IReadOnlyCollection<ApplicationIntegrationType> IntegrationTypes { get; }

    public IntegrationTypeAttribute(params ApplicationIntegrationType[] integrationTypes)
    {
        IntegrationTypes = integrationTypes?.ToImmutableArray()
                           ?? throw new ArgumentNullException(nameof(integrationTypes));
    }
}

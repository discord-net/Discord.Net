using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
        IntegrationTypes = integrationTypes?.Distinct().ToImmutableArray()
                           ?? throw new ArgumentNullException(nameof(integrationTypes));

        if (integrationTypes.Length == 0)
            throw new ArgumentException("A command must have at least one integration type.", nameof(integrationTypes));
    }
}

using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public class ApplicationCommandInteractionMetadata : IMessageInteractionMetadata
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong Id { get; }

    /// <inheritdoc />
    public InteractionType Type { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <inheritdoc />
    public ulong? OriginalResponseMessageId { get; }

    /// <summary>
    ///     
    /// </summary>
    public string Name { get; }

    internal ApplicationCommandInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, string name)
    {
        Id = id;
        Type = type;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
    }
}

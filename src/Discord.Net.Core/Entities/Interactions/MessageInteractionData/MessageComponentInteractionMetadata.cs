using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public readonly struct MessageComponentInteractionMetadata : IMessageInteractionMetadata
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
    public ulong InteractedMessageId { get; }

    internal MessageComponentInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, ulong interactedMessageId)
    {
        Id = id;
        Type = type;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
        InteractedMessageId = interactedMessageId;
    }
}


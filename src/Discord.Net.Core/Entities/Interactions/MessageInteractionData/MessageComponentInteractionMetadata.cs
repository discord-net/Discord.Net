using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the metadata of a component interaction.
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
    public ulong UserId { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <inheritdoc />
    public ulong? OriginalResponseMessageId { get; }

    /// <summary>
    ///     Gets the ID of the message that was interacted with to trigger the interaction.
    /// </summary>
    public ulong InteractedMessageId { get; }

    internal MessageComponentInteractionMetadata(ulong id, InteractionType type, ulong userId, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, ulong interactedMessageId)
    {
        Id = id;
        Type = type;
        UserId = userId;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
        InteractedMessageId = interactedMessageId;
    }
}


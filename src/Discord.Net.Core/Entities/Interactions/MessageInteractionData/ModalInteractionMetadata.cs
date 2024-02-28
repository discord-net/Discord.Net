using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public class ModalSubmitInteractionMetadata :IMessageInteractionMetadata
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
    public IMessageInteractionMetadata TriggeringInteractionMetadata { get; }

    internal ModalSubmitInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, IMessageInteractionMetadata triggeringInteractionMetadata)
    {
        Id = id;
        Type = type;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
        TriggeringInteractionMetadata = triggeringInteractionMetadata;
    }
}

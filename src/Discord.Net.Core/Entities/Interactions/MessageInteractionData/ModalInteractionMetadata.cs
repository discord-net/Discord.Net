using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the metadata of a modal interaction.
/// </summary>
public readonly struct ModalSubmitInteractionMetadata :IMessageInteractionMetadata
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
    public IUser User { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <inheritdoc />
    public ulong? OriginalResponseMessageId { get; }

    /// <summary>
    ///     Gets the interaction metadata of the interaction that responded with the modal.
    /// </summary>
    public IMessageInteractionMetadata TriggeringInteractionMetadata { get; }

    internal ModalSubmitInteractionMetadata(ulong id, InteractionType type, ulong userId, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, IMessageInteractionMetadata triggeringInteractionMetadata, IUser user)
    {
        Id = id;
        Type = type;
        UserId = userId;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
        TriggeringInteractionMetadata = triggeringInteractionMetadata;
        User = user;
    }
}

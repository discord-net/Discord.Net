using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public class MessageComponentInteractionMetadata : BaseMessageInteractionMetadata
{
    internal MessageComponentInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, ulong interactedMessageId) : base(id, type, integrationOwners, originalResponseMessageId)
        => InteractedMessageId = interactedMessageId;

    /// <summary>
    ///     
    /// </summary>
    public ulong InteractedMessageId { get; }
}

